using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class Animator : AnimationControl
    {
        [CanBeNull]
        public Godot.Animation Animation
        {
            get => _animation.Value;
            set => _animation.Value = value;
        }

        public IObservable<Godot.Animation> OnAnimationChange => _animation;

        protected AnimationNodeAnimation Node { get; }

        private readonly ReactiveProperty<Godot.Animation> _animation;

        public Animator(
            [NotNull] AnimationNodeAnimation node, AnimationGraphContext context) : base(context)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            Node = node;

            var currentAnim = Node.Animation;

            _animation = new ReactiveProperty<Godot.Animation>(
                string.IsNullOrEmpty(currentAnim) ? null : context.Player.GetAnimation(currentAnim));

            _animation
                .Select(context.Player.AddAnimation)
                .Subscribe(Node.SetAnimation);
        }

        public override void Dispose()
        {
            _animation?.Dispose();
        }

        public static Animator Create(
            [NotNull] string name,
            [NotNull] IAnimationGraph parent,
            [NotNull] AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(context, nameof(context));

            return !(parent.GetAnimationNode(name) is AnimationNodeAnimation node) ? null : new Animator(node, context);
        }
    }

    public static class AnimatorExtensions
    {
        [CanBeNull]
        public static Animator GetAnimator(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantControl(path) as Animator;
        }
    }
}
