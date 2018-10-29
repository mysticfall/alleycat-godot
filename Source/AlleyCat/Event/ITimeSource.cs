using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EnsureThat;

namespace AlleyCat.Event
{
    public interface ITimeSource
    {
        IObservable<float> OnProcess { get; }

        IObservable<float> OnPhysicsProcess { get; }

        IScheduler Scheduler { get; }

        IScheduler PhysicsScheduler { get; }
    }

    public static class TimeSourceExtensions
    {
        public static IObservable<float> OnProcess(this ITimeSource source, ProcessMode mode)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            switch (mode)
            {
                case ProcessMode.Disable:
                    return Observable.Empty<float>();
                case ProcessMode.Idle:
                    return source.OnProcess;
                case ProcessMode.Physics:
                    return source.OnPhysicsProcess;
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
                    return source.Scheduler;
                case ProcessMode.Physics:
                    return source.PhysicsScheduler;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class DummyScheduler : IScheduler
        {
            public DateTimeOffset Now => DateTimeOffset.Now;

            public IDisposable Schedule<TState>(
                TState state, Func<IScheduler, TState, IDisposable> action) => Disposable.Empty;

            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime,
                Func<IScheduler, TState, IDisposable> action) => Disposable.Empty;

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime,
                Func<IScheduler, TState, IDisposable> action) => Disposable.Empty;
        }
    }
}
