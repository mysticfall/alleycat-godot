using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public interface IDisposableCollector
    {
        void Collect(IDisposable disposable);
    }

    public static class DisposableExtensions
    {
        private const string NodeName = "DisposableCollector";

        public static IDisposableCollector AsDisposableCollector(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node as IDisposableCollector ?? node.GetComponent(NodeName, _ => new BaseNode(NodeName));
        }

        public static T DisposeWith<T>(this T disposable, IDisposableCollector collector)
            where T : class, IDisposable
        {
            Ensure.That(collector, nameof(collector)).IsNotNull();

            collector.Collect(disposable);

            return disposable;
        }

        public static T DisposeWith<T>(this T disposable, Node node) where T : IDisposable
        {
            node.AsDisposableCollector().Collect(disposable);

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
}
