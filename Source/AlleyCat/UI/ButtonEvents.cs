using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public interface IButtonEvent : IEvent<Button>
    {
    }

    public struct ButtonPressedEvent : IButtonEvent
    {
        public Button Source { get; }

        public ButtonPressedEvent(Button source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }
}
