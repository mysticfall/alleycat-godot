using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace AlleyCat.Common
{
    public class NodeStore<T> : Object
    {
        private readonly IDictionary<int, T> _store = new Dictionary<int, T>();

        public Option<T> Find(Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return _store.TryGetValue(node.GetInstanceId());
        }

        public T Get(Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return _store[node.GetInstanceId()];
        }

        public T Get(Node node, Func<Node, T> factory) => Find(node).IfNone(() =>
        {
            Ensure.That(factory, nameof(factory)).IsNotNull();

            var data = factory(node);

            Debug.Assert(data != null, "data != null");

            node.Connect("tree_exited", this, nameof(OnNodeExited), new Array {node});

            _store.Add(node.GetInstanceId(), data);

            return data;
        });

        [UsedImplicitly]
        protected virtual void OnNodeExited(Node node)
        {
            Find(node).OfType<IDisposable>().Iter(d => d.Dispose());

            _store.Remove(node.GetInstanceId());
        }

        protected override void Dispose(bool disposing)
        {
            _store.Clear();

            base.Dispose(disposing);
        }
    }
}
