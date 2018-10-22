using System;
using System.Reactive;
using System.Reactive.Linq;
using AlleyCat.Common;
using EnsureThat;

namespace AlleyCat.Event
{
    public static class ObservableExtensions
    {
        public static IObservable<Unit> AsUnitObservable<T>(this IObservable<T> source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return source.Select(_ => Unit.Default);
        }

        public static IObservable<Tuple<T, T>> Pairwise<T>(this IObservable<T> source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return source.Scan(
                Tuple.Create(default(T), default(T)),
                (agg, current) => Tuple.Create(agg.Item2, current));
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return new ReactiveProperty<T>(source);
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return new ReactiveProperty<T>(source, initialValue);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return new ReadOnlyReactiveProperty<T>(source);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
            this IObservable<T> source, T initialValue)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        public static void CompleteAndDispose<T>(this IObserver<T> subject)
        {
            Ensure.That(subject, nameof(subject)).IsNotNull();

            try
            {
                subject.OnCompleted();
            }
            finally
            {
                (subject as IDisposable)?.DisposeQuietly();
            }
        }
    }
}
