using System.Reactive.Subjects;

namespace AlleyCat.Event
{
    public interface IReactiveObject : ILifecycleAware
    {
        ISubject<T> CreateSubject<T>();

        BehaviorSubject<T> CreateSubject<T>(T initialValue);
    }
}
