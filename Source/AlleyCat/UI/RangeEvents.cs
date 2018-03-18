using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public interface IRangeEvent : IEvent<Range>
    {
    }

    public struct ValueChangedEvent : IRangeEvent
    {
        public float Value { get; }

        public Range Source { get; }

        public ValueChangedEvent(float value, [NotNull] Range source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Value = value;
            Source = source;
        }
    }
}
