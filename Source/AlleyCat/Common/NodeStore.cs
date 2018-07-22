using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AlleyCat.Common
{
    public class NodeStore<T> : Object
    {
        private readonly IDictionary<int, T> _store = new Dictionary<int, T>();

        [CanBeNull]
        public T Get([NotNull] Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return !_store.TryGetValue(node.GetInstanceId(), out var data) ? default : data;
        }

        [CanBeNull]
        public T GetOrCreate([NotNull] Node node, [NotNull] Func<Node, T> factory)
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(factory, nameof(factory));

            if (_store.TryGetValue(node.GetInstanceId(), out var data))
            {
                return data;
            }

            data = factory(node);

            node.Connect("tree_exited", this, nameof(OnNodeExited), new object[] {node});

            _store.Add(node.GetInstanceId(), data);

            return data;
        }

        [UsedImplicitly]
        protected virtual void OnNodeExited([NotNull] Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            if (!_store.TryGetValue(node.GetInstanceId(), out var data))
            {
                return;
            }

            (data as IDisposable)?.Dispose();

            _store.Remove(node.GetInstanceId());
        }

        protected override void Dispose(bool disposing)
        {
            _store?.Clear();

            base.Dispose(disposing);
        }
    }
}
