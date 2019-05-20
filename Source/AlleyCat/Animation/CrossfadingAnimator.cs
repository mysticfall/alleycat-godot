using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class CrossfadingAnimator : AnimationControl, IAnimator
    {
        public Option<Godot.Animation> Animation
        {
            get => _animation.Value;
            set => _animation.OnNext(value);
        }

        public float Time
        {
            get => TransitionNode.XfadeTime;
            set => TransitionNode.XfadeTime = Mathf.Max(value, 0);
        }

        public IObservable<Option<Godot.Animation>> OnAnimationChange => _animation.AsObservable();

        protected string Parameter { get; }

        protected int Transition => (int) Context.AnimationTree.Get(Parameter);

        protected AnimationNodeTransition TransitionNode { get; }

        protected AnimationNodeAnimation AnimationNode => Transition == 0 ? AnimationNode1 : AnimationNode2;

        protected AnimationNodeAnimation AnimationNode1 { get; }

        protected AnimationNodeAnimation AnimationNode2 { get; }

        private readonly BehaviorSubject<Option<Godot.Animation>> _animation;

        public CrossfadingAnimator(
            string key,
            string parameter,
            AnimationNodeTransition transitionNode,
            AnimationNodeAnimation animationNode1,
            AnimationNodeAnimation animationNode2,
            AnimationGraphContext context) : base(key, context)
        {
            Ensure.That(parameter, nameof(parameter)).IsNotNull();
            Ensure.That(transitionNode, nameof(transitionNode)).IsNotNull();
            Ensure.That(animationNode1, nameof(animationNode1)).IsNotNull();
            Ensure.That(animationNode2, nameof(animationNode2)).IsNotNull();

            Parameter = parameter;

            TransitionNode = transitionNode;
            AnimationNode1 = animationNode1;
            AnimationNode2 = animationNode2;

            var current = AnimationNode.Animation.TrimToOption().Bind(context.Player.FindAnimation);

            _animation = CreateSubject(current);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnAnimationChange
                .DistinctUntilChanged()
                .Select(a => a.Map(Context.Player.AddAnimation))
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(animation =>
                {
                    var next = Transition == 0 ? 1 : 0;
                    var node = next == 0 ? AnimationNode1 : AnimationNode2;

                    this.LogDebug("Crossfading animation from '{}' to '{}'.",
                        fun(() => AnimationNode.Animation),
                        animation);

                    node.Animation = animation.ValueUnsafe();

                    Context.AnimationTree.Set(Parameter, next);
                }, this);
        }

        public static Option<CrossfadingAnimator> TryCreate(
            string name,
            IAnimationGraph parent,
            AnimationGraphContext context)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            return (from transition in parent.FindAnimationNode<AnimationNodeTransition>(name)
                from animation1 in parent.FindAnimationNode<AnimationNodeAnimation>(name + " Animation 1")
                from animation2 in parent.FindAnimationNode<AnimationNodeAnimation>(name + " Animation 2")
                select (transition, animation1, animation2)).Map(t =>
            {
                var key = string.Join(":", parent.Key, name);
                var parameter = string.Join("/",
                    new[] {"parameters", parent.Key, name, "current"}.Where(v => v.Length > 0));

                return new CrossfadingAnimator(
                    key, parameter, t.transition, t.animation1, t.animation2, context);
            });
        }
    }

    public static class CrossfadingAnimatorExtensions
    {
        public static Option<CrossfadingAnimator> FindCrossfadingAnimator(
            this IAnimationGraph graph, string path)
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();

            return graph.FindDescendantControl<CrossfadingAnimator>(path);
        }
    }
}
