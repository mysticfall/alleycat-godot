namespace AlleyCat.Event
{
    public interface IEvent<out T>
    {
        T Source { get; }
    }
}
