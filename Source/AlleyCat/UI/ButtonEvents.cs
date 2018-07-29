using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public interface IButtonEvent : IEvent<Button>
    {
    }

    public struct ButtonPressedEvent : IButtonEvent
    {
        public Button Source { get; }

        public ButtonPressedEvent([NotNull] Button source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Source = source;
        }
    }
}
