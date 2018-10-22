using AlleyCat.Event;
using EnsureThat;

namespace AlleyCat.UI
{
    public interface IControlEvent : IEvent<Godot.Control>
    {
    }

    public struct MouseEnteredEvent : IControlEvent
    {
        public Godot.Control Source { get; }

        public MouseEnteredEvent(Godot.Control source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }

    public struct MouseExitedEvent : IControlEvent
    {
        public Godot.Control Source { get; }

        public MouseExitedEvent(Godot.Control source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }
}
