using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Tuple = System.Tuple;
using Unit = System.Reactive.Unit;

namespace AlleyCat.Event
{
    public static class ObservableExtensions
    {
        private static readonly System.Action NoOp = () => { };

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

        public static void Subscribe<T>(
            this IObservable<T> observable,
            IDisposableCollector collector,
            bool resubscribe = true)
        {
            Subscribe(
                observable,
                _ => { },
                NoOp,
                Some(collector).OfType<ILoggable>().HeadOrNone(),
                collector,
                resubscribe);
        }

        public static void Subscribe<T>(
            this IObservable<T> observable,
            Node node, bool
                resubscribe = true)
        {
            Subscribe(
                observable,
                _ => { },
                NoOp,
                Some(node).OfType<ILoggable>().HeadOrNone(),
                node.AsDisposableCollector(),
                resubscribe);
        }

        public static void Subscribe<T>(
            this IObservable<T> observable,
            Action<T> onNext,
            IDisposableCollector collector,
            bool resubscribe = true)
        {
            Subscribe(
                observable,
                onNext,
                NoOp,
                Some(collector).OfType<ILoggable>().HeadOrNone(),
                collector,
                resubscribe);
        }

        public static void Subscribe<T>(
            this IObservable<T> observable,
            Action<T> onNext,
            Node node,
            bool resubscribe = true)
        {
            Subscribe(
                observable,
                onNext,
                NoOp,
                Some(node).OfType<ILoggable>().HeadOrNone(),
                node.AsDisposableCollector(),
                resubscribe);
        }

        public static void Subscribe<T>(
            this IObservable<T> observable,
            Action<T> onNext,
            System.Action onCompleted,
            IDisposableCollector collector,
            bool resubscribe = true)
        {
            Subscribe(
                observable,
                onNext,
                onCompleted,
                Some(collector).OfType<ILoggable>().HeadOrNone(),
                collector,
                resubscribe);
        }

        public static void Subscribe<T>(
            this IObservable<T> observable,
            Action<T> onNext,
            System.Action onCompleted,
            Node node,
            bool resubscribe = true)
        {
            Subscribe(
                observable,
                onNext,
                onCompleted,
                Some(node).OfType<ILoggable>().HeadOrNone(),
                node.AsDisposableCollector(),
                resubscribe);
        }

        private static void Subscribe<T>(
            this IObservable<T> observable,
            Action<T> onNext,
            System.Action onCompleted,
            Option<ILoggable> logger,
            IDisposableCollector collector,
            bool resubscribe = true)
        {
            Ensure.That(observable, nameof(observable)).IsNotNull();
            Ensure.That(onNext, nameof(onNext)).IsNotNull();

            collector.Collect(observable.Subscribe(onNext, OnError, onCompleted));

            void OnError(Exception e)
            {
                logger.BiIter(
                    l => l.LogError(e, "Subscription terminated with an error."),
                    () =>
                    {
                        GD.Print("ERROR - Subscription terminated with an error.");
                        GD.Print(e.ToString());
                    });

                if (resubscribe)
                {
                    Subscribe(observable, onNext, onCompleted, logger, collector);
                }
            }
        }

        public static void CompleteAndDispose<T>(this Subject<T> subject)
        {
            Ensure.That(subject, nameof(subject)).IsNotNull();

            try
            {
                subject.OnCompleted();
            }
            finally
            {
                subject.DisposeQuietly();
            }
        }
    }
}
