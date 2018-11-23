using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AlleyCat.Autowire
{
    internal class DependencyChain : ICollection<DependencyNode>
    {
        public bool IsReadOnly => false;

        public int Count => _nodes.Count;

        private readonly List<DependencyNode> _nodes = new List<DependencyNode>();

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
            _nodes.Add(item);

            _dirty = true;
        }

        public bool Contains(DependencyNode item) => _nodes.Contains(item);

        public bool Remove(DependencyNode item)
        {
            var removed = _nodes.Remove(item);

            if (removed)
            {
                foreach (var node in _nodes)
                {
                    node.Dependencies.Remove(item);
                }
            }

            _dirty |= removed;

            return removed;
        }

        public void Clear()
        {
            _nodes.Clear();

            _dirty = false;
        }

        public void CopyTo(DependencyNode[] array, int arrayIndex) => _nodes.CopyTo(array, arrayIndex);

        private void UpdateDependencies()
        {
            var tuples =
                from source in _nodes
                from target in _nodes
                where source.Instance != target.Instance
                where source.Provides.Any(target.Requires.Contains)
                select (source, target);

            tuples.Iter(t => t.target.AddDependency(t.source));

            _nodes.Sort();

            _dirty = false;
        }
    }
}
