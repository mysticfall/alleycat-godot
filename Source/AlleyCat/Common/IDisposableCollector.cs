using System;
using System.Diagnostics;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public interface IDisposableCollector
    {
        void Collect(IDisposable disposable);
    }

    public static class DisposableExtensions
    {
        private const string NodeName = "DisposableCollector";

        public static T DisposeWith<T>(this T disposable, IDisposableCollector collector)
            where T : class, IDisposable
        {
            Ensure.That(collector, nameof(collector)).IsNotNull();

            collector.Collect(disposable);

            return disposable;
        }

        public static T DisposeWith<T>(this T disposable, Node node)
            where T : class, IDisposable
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            if (node is IDisposableCollector collector)
            {
                collector.Collect(disposable);
            }
            else
            {
                node.GetComponent(NodeName, _ => new BaseNode(NodeName)).Collect(disposable);
            }

            return disposable;
        }

        public static void DisposeQuietly(this IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    internal struct ObserverDisposer<TObserver, TValue> : IDisposable
        where TObserver : IObserver<TValue>
    {
        private Option<TObserver> _observer;

        public ObserverDisposer(TObserver observer)
        {
            Debug.Assert(observer != null, "observer != null");

            _observer = Some(observer);
        }

        public void Dispose()
        {
            _observer.Iter(o => o.CompleteAndDispose());
            _observer = None;
        }
    }
}
