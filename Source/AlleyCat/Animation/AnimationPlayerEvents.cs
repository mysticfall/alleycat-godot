using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationPlayerEvent : IEvent<AnimationPlayer>
    {
        [NotNull]
        string Animation { get; }
    }

    public struct AnimationStartEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        public AnimationPlayer Source { get; }

        public AnimationStartEvent([NotNull] string animation, [NotNull] AnimationPlayer source)
        {
            Ensure.String.IsNotNullOrWhiteSpace(animation, nameof(animation));
            Ensure.Any.IsNotNull(source, nameof(source));

            Animation = animation;
            Source = source;
        }
    }

    public struct AnimationFinishEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        public AnimationPlayer Source { get; }

        public AnimationFinishEvent([NotNull] string animation, [NotNull] AnimationPlayer source)
        {
            Ensure.String.IsNotNullOrWhiteSpace(animation, nameof(animation));
            Ensure.Any.IsNotNull(source, nameof(source));

            Animation = animation;
            Source = source;
        }
    }

    public struct AnimationChangeEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        [NotNull]
        public string OldAnimation { get; }

        public AnimationPlayer Source { get; }

        public AnimationChangeEvent(
            [NotNull] string animation,
            [NotNull] string oldAnimation,
            [NotNull] AnimationPlayer source)
        {
            Ensure.String.IsNotNullOrWhiteSpace(animation, nameof(animation));
            Ensure.String.IsNotNullOrWhiteSpace(oldAnimation, nameof(oldAnimation));

            Ensure.Any.IsNotNull(source, nameof(source));

            Animation = animation;
            OldAnimation = oldAnimation;
            Source = source;
        }
    }
}
