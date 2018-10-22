using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public interface IColorPickerButtonEvent : IEvent<ColorPickerButton>
    {
    }

    public struct ColorChangedEvent : IColorPickerButtonEvent
    {
        public Color Color { get; }

        public ColorPickerButton Source { get; }

        public ColorChangedEvent(Color color, ColorPickerButton source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Color = color;
            Source = source;
        }
    }
}
