using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using EnsureThat;

namespace AlleyCat.Event
{
    public interface ITimeSource
    {
        IObservable<float> OnIdleLoop { get; }

        IObservable<float> OnPhysicsLoop { get; }

        IScheduler IdleScheduler { get; }

        IScheduler PhysicsScheduler { get; }
    }

    public static class TimeSourceExtensions
    {
        public static IObservable<float> OnLoop(this ITimeSource source, ProcessMode mode)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            switch (mode)
            {
                case ProcessMode.Disable:
                    return Observable.Empty<float>();
                case ProcessMode.Idle:
                    return source.OnIdleLoop;
                case ProcessMode.Physics:
                    return source.OnPhysicsLoop;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IScheduler Scheduler(this ITimeSource source, ProcessMode mode)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            switch (mode)
            {
                case ProcessMode.Disable:
                    return new DummyScheduler();
                case ProcessMode.Idle:
                    return source.IdleScheduler;
                case ProcessMode.Physics:
                    return source.PhysicsScheduler;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
