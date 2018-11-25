using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

namespace AlleyCat.Animation
{
    public class Animator : AnimationControl, IAnimator
    {
        public Option<Godot.Animation> Animation
        {
            get => _animation.Value;
            set => _animation.OnNext(value);
        }

        public IObservable<Option<Godot.Animation>> OnAnimationChange => _animation.AsObservable();

        protected AnimationNodeAnimation Node { get; }

        private readonly BehaviorSubject<Option<Godot.Animation>> _animation;

        public Animator(string key, AnimationNodeAnimation node, AnimationGraphContext context) : base(key, context)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Node = node;

            var current = Node.Animation.TrimToOption().Bind(context.Player.FindAnimation);

            _animation = new BehaviorSubject<Option<Godot.Animation>>(current).DisposeWith(this);

            _animation
                .Select(a => a.Map(context.Player.AddAnimation).ValueUnsafe())
                .Do(a => this.LogDebug("Animation has changed to '{}'.", a))
                .Subscribe(Node.SetAnimation)
                .DisposeWith(this);
        }

        public static Option<Animator> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            return parent
                .FindAnimationNode<AnimationNodeAnimation>(name)
                .Map(n => new Animator(string.Join(":", parent.Key, name), n, context))
                .HeadOrNone();
        }
    }
}
