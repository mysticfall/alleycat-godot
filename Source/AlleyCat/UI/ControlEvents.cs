using AlleyCat.Event;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public interface IControlEvent : IEvent<Godot.Control>
    {
    }

    public struct MouseEnteredEvent : IControlEvent
    {
        public Godot.Control Source { get; }

        public MouseEnteredEvent([NotNull] Godot.Control source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Source = source;
        }
    }

    public struct MouseExitedEvent : IControlEvent
    {
        public Godot.Control Source { get; }

        public MouseExitedEvent([NotNull] Godot.Control source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Source = source;
        }
    }
}
