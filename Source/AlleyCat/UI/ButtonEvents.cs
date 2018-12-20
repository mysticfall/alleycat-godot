using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public interface IButtonEvent : IEvent<BaseButton>
    {
    }

    public struct ButtonPressedEvent : IButtonEvent
    {
        public BaseButton Source { get; }

        public ButtonPressedEvent(BaseButton source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }

    public struct ButtonUpEvent : IButtonEvent
    {
        public BaseButton Source { get; }

        public ButtonUpEvent(BaseButton source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }

    public struct ButtonDownEvent : IButtonEvent
    {
        public BaseButton Source { get; }

        public ButtonDownEvent(BaseButton source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }
}
