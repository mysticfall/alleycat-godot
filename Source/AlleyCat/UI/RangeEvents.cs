using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public interface IRangeEvent : IEvent<Range>
    {
    }

    public struct ValueChangedEvent : IRangeEvent
    {
        public float Value { get; }

        public Range Source { get; }

        public ValueChangedEvent(float value, Range source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Value = value;
            Source = source;
        }
    }
}
