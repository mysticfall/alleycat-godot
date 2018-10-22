using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IAnimationPlayerEvent : IEvent<AnimationPlayer>
    {
    }

    public struct AnimationStartEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        public AnimationPlayer Source { get; }

        public AnimationStartEvent(string animation, AnimationPlayer source)
        {
            Ensure.That(animation, nameof(animation)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Animation = animation;
            Source = source;
        }
    }

    public struct AnimationFinishEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        public AnimationPlayer Source { get; }

        public AnimationFinishEvent(string animation, AnimationPlayer source)
        {
            Ensure.That(animation, nameof(animation)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Animation = animation;
            Source = source;
        }
    }

    public struct AnimationChangeEvent : IAnimationPlayerEvent
    {
        public Option<string> Animation { get; }

        public Option<string> OldAnimation { get; }

        public AnimationPlayer Source { get; }

        public AnimationChangeEvent(
            Option<string> animation, Option<string> oldAnimation, AnimationPlayer source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Animation = animation;
            OldAnimation = oldAnimation;
            Source = source;
        }
    }
}
