using System;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class DisposableExtensions
    {
        public static void AddTo([NotNull] this IDisposable disposable, [NotNull] Node node)
        {
            Ensure.Any.IsNotNull(disposable, nameof(disposable));
            Ensure.Any.IsNotNull(node, nameof(node));

            node.GetOrCreateChild(_ => new DisposableCollector()).Add(disposable);
        }
    }
}
