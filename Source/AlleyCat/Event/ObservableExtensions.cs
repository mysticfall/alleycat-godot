using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Event
{
    public static class ObservableExtensions
    {
        public static IObservable<Tuple<T, T>> Pairwise<T>(this IObservable<T> source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return source.Scan(
                Tuple.Create(default(T), default(T)),
                (agg, current) => Tuple.Create(agg.Item2, current));
        }

        public static IObservable<Unit> AsUnitObservable<T>(this IObservable<T> source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            return source.Select(_ => Unit.Default);
        }

        public static void CompleteAndDispose<T>(this ISubject<T> subject)
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
