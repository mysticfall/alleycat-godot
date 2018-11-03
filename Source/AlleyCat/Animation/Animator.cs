using System;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
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
            set => _animation.Value = value;
        }

        public IObservable<Option<Godot.Animation>> OnAnimationChange => _animation.AsObservable();

        protected AnimationNodeAnimation Node { get; }

        private readonly ReactiveProperty<Option<Godot.Animation>> _animation;

        public Animator(AnimationNodeAnimation node, AnimationGraphContext context) : base(context)
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            Node = node;

            var current = Node.Animation.TrimToOption().Bind(context.Player.FindAnimation);

            _animation = new ReactiveProperty<Option<Godot.Animation>>(current).AddTo(this);

            _animation
                .Select(a => a.Map(context.Player.AddAnimation).ValueUnsafe())
                .Subscribe(Node.SetAnimation)
                .AddTo(this);
        }

        public static Option<Animator> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            return parent
                .FindAnimationNode<AnimationNodeAnimation>(name)
                .Map(n => new Animator(n, context)).HeadOrNone();
        }
    }
}
