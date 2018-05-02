using System;
using System.Reactive;
using System.Reactive.Linq;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public static class ObservableExtensions
    {
        [NotNull]
        public static IObservable<Unit> AsUnitObservable<T>([NotNull] this IObservable<T> source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return source.Select(_ => Unit.Default);
        }

        [NotNull]
        public static IObservable<Tuple<T, T>> Pairwise<T>([NotNull] this IObservable<T> source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return source.Scan(
                Tuple.Create(default(T), default(T)), (agg, current) => Tuple.Create(agg.Item2, current));
        }

        [NotNull]
        public static ReactiveProperty<T> ToReactiveProperty<T>([NotNull] this IObservable<T> source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return new ReactiveProperty<T>(source);
        }

        [NotNull]
        public static ReactiveProperty<T> ToReactiveProperty<T>(
            [NotNull] this IObservable<T> source, T initialValue)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return new ReactiveProperty<T>(source, initialValue);
        }

        [NotNull]
        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
            [NotNull] this IObservable<T> source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return new ReadOnlyReactiveProperty<T>(source);
        }

        [NotNull]
        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
            [NotNull] this IObservable<T> source, T initialValue)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }
    }
}
