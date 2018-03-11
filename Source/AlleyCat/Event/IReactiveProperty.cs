namespace AlleyCat.Event
{
    /// <summary>
    /// Adapted from UniRx (https://github.com/neuecc/UniRx).
    /// </summary>
    public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }
}
