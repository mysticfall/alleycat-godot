using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class CrossfadingAnimator : AnimationControl, IAnimator
    {
        [CanBeNull]
        public Godot.Animation Animation
        {
            get => _animation.Value;
            set => _animation.Value = value;
        }

        public float Time
        {
            get => TransitionNode.XfadeTime;
            set => TransitionNode.XfadeTime = value;
        }

        public IObservable<Godot.Animation> OnAnimationChange => _animation;

        protected string Parameter { get; }

        protected int Transition => (int) Context.AnimationTree.Get(Parameter);

        protected AnimationNodeTransition TransitionNode { get; }

        protected AnimationNodeAnimation AnimationNode => Transition == 1 ? AnimationNode1 : AnimationNode2;

        protected AnimationNodeAnimation AnimationNode1 { get; }

        protected AnimationNodeAnimation AnimationNode2 { get; }

        private readonly ReactiveProperty<Godot.Animation> _animation;

        public CrossfadingAnimator(
            [NotNull] string parameter,
            [NotNull] AnimationNodeTransition transitionNode,
            [NotNull] AnimationNodeAnimation animationNode1,
            [NotNull] AnimationNodeAnimation animationNode2,
            AnimationGraphContext context) : base(context)
        {
            Ensure.Any.IsNotNull(parameter, nameof(parameter));
            Ensure.Any.IsNotNull(transitionNode, nameof(transitionNode));
            Ensure.Any.IsNotNull(animationNode1, nameof(animationNode1));
            Ensure.Any.IsNotNull(animationNode2, nameof(animationNode2));

            Parameter = parameter;

            TransitionNode = transitionNode;
            AnimationNode1 = animationNode1;
            AnimationNode2 = animationNode2;

            var currentAnim = AnimationNode.Animation;

            _animation = new ReactiveProperty<Godot.Animation>(
                string.IsNullOrEmpty(currentAnim) ? null : context.Player.GetAnimation(currentAnim));

            _animation
                .Select(context.Player.AddAnimation)
                .DistinctUntilChanged()
                .Subscribe(animation =>
                {
                    var next = Transition == 1 ? 2 : 1;
                    var node = next == 1 ? AnimationNode1 : AnimationNode2;

                    node.SetAnimation(animation);

                    Context.AnimationTree.Set(Parameter, next);
                });
        }

        public override void Dispose()
        {
            _animation?.Dispose();
        }

        public static CrossfadingAnimator Create(
            [NotNull] string name,
            [NotNull] IAnimationGraph parent,
            [NotNull] AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(context, nameof(context));

            var transitionNode = parent.GetAnimationNode(name) as AnimationNodeTransition;

            if (transitionNode == null) return null;

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            var animationNode1 = parent.GetAnimationNode(name + " Animation 1") as AnimationNodeAnimation;
            var animationNode2 = parent.GetAnimationNode(name + " Animation 2") as AnimationNodeAnimation;

            if (animationNode1 == null || animationNode2 == null) return null;

            var parameter = string.Join("/",
                new[] {"parameters", parent.Path, name, "current"}.Where(v => v.Length > 0));

            return new CrossfadingAnimator(parameter, transitionNode, animationNode1, animationNode2, context);
        }
    }

    public static class CrossfadingAnimatorExtensions
    {
        [CanBeNull]
        public static CrossfadingAnimator GetCrossfadingAnimator(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantControl(path) as CrossfadingAnimator;
        }
    }
}
