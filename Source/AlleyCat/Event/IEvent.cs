using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public interface IEvent<out T>
    {
        [NotNull]
        T Source { get; }
    }
}
