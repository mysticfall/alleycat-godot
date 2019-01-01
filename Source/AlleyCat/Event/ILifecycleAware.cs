using System;
using AlleyCat.Autowire;

namespace AlleyCat.Event
{
    [NonInjectable]
    public interface ILifecycleAware : IDisposable
    {
        IObservable<bool> Initialized { get; }

        IObservable<bool> Disposed { get; }
    }
}
