using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;

namespace AlleyCat.Autowire
{
    internal class DependencyChain : ICollection<DependencyNode>
    {
        public bool IsReadOnly => false;

        public int Count => _nodes.Count;

        private Lst<DependencyNode> _nodes;

        private bool _dirty;

        public IEnumerator<DependencyNode> GetEnumerator()
        {
            if (_dirty)
            {
                UpdateDependencies();
            }

            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(DependencyNode item)
        {
            _nodes += item;

            _dirty = true;
        }

        public bool Contains(DependencyNode item) => _nodes.Contains(item);

        public bool Remove(DependencyNode item)
        {
            var count = _nodes.Count;

            _nodes = _nodes.Remove(item);

            var removed = count != _nodes.Count;

            if (removed)
            {
                _nodes.Iter(n => n.Dependencies.Remove(item));
            }

            _dirty |= removed;

            return removed;
        }

        public void Clear()
        {
            _nodes = _nodes.Clear();

            _dirty = false;
        }

        public void CopyTo(DependencyNode[] array, int arrayIndex) =>
            Enumerable.ToArray(_nodes).CopyTo(array, arrayIndex);

        public void UpdateDependencies()
        {
            _nodes.Iter(n => n.ClearDependencies());

            var tuples =
                from source in _nodes
                from target in _nodes
                where source.Instance != target.Instance
                where source.Provides.Any(target.Requires.Contains)
                select (source, target);

            tuples.Iter(t => t.target.AddDependency(t.source));

            _nodes = Sort().Reverse();

            _dirty = false;
        }

        private Lst<DependencyNode> Sort() => Sort(Lst<DependencyNode>.Empty);

        private Lst<DependencyNode> Sort(Lst<DependencyNode> sorted)
        {
            return _nodes.Find(n => n.SortMark != SortMark.Permanent).Match(
                unmarked => Sort(Mark(unmarked, sorted)),
                () => sorted);
        }

        private Lst<DependencyNode> Mark(DependencyNode node, Lst<DependencyNode> sorted)
        {
            if (node.SortMark != SortMark.Unmarked) return sorted;

            node.SortMark = SortMark.Temporary;

            var result = _nodes.Filter(node.DependsOn).Fold(sorted, (s, n) => Mark(n, s));

            node.SortMark = SortMark.Permanent;

            return node + result;
        }
    }
}
