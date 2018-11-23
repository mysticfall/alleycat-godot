using System.Collections;
using System.Collections.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    internal struct TreeItemChildren : IEnumerable<TreeItem>, IEnumerator<TreeItem>
    {
        public TreeItem Current { get; private set; }

        object IEnumerator.Current => Current;

        public IEnumerator<TreeItem> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly TreeItem _parent;

        private bool _initial;

        public TreeItemChildren(TreeItem parent)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            _parent = parent;

            Current = null;
            _initial = true;
        }

        public bool MoveNext()
        {
            Current = _initial ? _parent.GetChildren() : Current?.GetNext();
            _initial = false;

            return Current != null;
        }

        public void Reset()
        {
            Current = null;
            _initial = true;
        }

        public void Dispose()
        {
        }
    }

    public static class TreeItemExtensions
    {
        public static IEnumerable<TreeItem> Children(this TreeItem parent) => new TreeItemChildren(parent);
    }
}
