using AlleyCat.Event;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public struct AnimationEvent : IEvent<IAnimationManager>
    {
        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public string Argument { get; }

        public IAnimationManager Source { get; }

        public AnimationEvent(
            [NotNull] string name, [CanBeNull] string argument, [NotNull] IAnimationManager source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Name = name;
            Argument = argument;
            Source = source;
        }
    }
}
