using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace AlleyCat.Event
{
    public class DummyScheduler : IScheduler
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
