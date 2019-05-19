using System.Reactive.Subjects;
using AlleyCat.Autowire;

namespace AlleyCat.Event
{
    [NonInjectable]
    public interface IReactiveObject : ILifecycleAware
    {
        ISubject<T> CreateSubject<T>();

        BehaviorSubject<T> CreateSubject<T>(T initialValue);
    }
}
