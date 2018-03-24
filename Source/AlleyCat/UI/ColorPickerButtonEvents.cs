using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public interface IColorPickerButtonEvent : IEvent<ColorPickerButton>
    {
    }

    public struct ColorChangedEvent : IColorPickerButtonEvent
    {
        public Color Color { get; }

        public ColorPickerButton Source { get; }

        public ColorChangedEvent(Color color, [NotNull] ColorPickerButton source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Color = color;
            Source = source;
        }
    }
}
