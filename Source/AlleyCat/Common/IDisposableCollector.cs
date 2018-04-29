using System;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IDisposableCollector
    {
        void Collect([NotNull] IDisposable disposable);
    }

    public static class DisposableExtensions
    {
        private const string NodeName = "DisposableCollector";

        [NotNull]
        public static T AddTo<T>([NotNull] this T disposable, [NotNull] Node node)
            where T : IDisposable
        {
            Ensure.Any.IsNotNull(disposable, nameof(disposable));
            Ensure.Any.IsNotNull(node, nameof(node));

            if (node is IDisposableCollector collector)
            {
                collector.Collect(disposable);
            }
            else
            {
                node.GetOrCreateChild(_ => new BaseNode(NodeName)).Collect(disposable);
            }

            return disposable;
        }
    }
}
