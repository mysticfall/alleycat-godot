using System;
using System.Reactive.Concurrency;
using AlleyCat.Common;

namespace AlleyCat.Event
{
    public interface ITimeSource
    {
        ProcessMode ProcessMode { get; }

        IObservable<float> OnLoop { get; }

        IScheduler Scheduler { get; }
    }
}
