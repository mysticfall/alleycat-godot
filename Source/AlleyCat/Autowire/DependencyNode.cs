using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    internal struct DependencyNode : IDependencyNode, IComparable<DependencyNode>
    {
        [NotNull]
        public Node Instance { get; }

        [NotNull]
        public IEnumerable<INodeProcessor> Processors { get; }

        [NotNull]
        public ISet<DependencyNode> Dependencies { get; }

        public ISet<Type> Requires { get; }

        public ISet<Type> Provides { get; }

        public DependencyNode([NotNull] Node node, ServiceDefinition definition)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            Instance = node;

            Processors = definition.Processors;
            Dependencies = new HashSet<DependencyNode>();

            Requires = definition.Requires;
            Provides = definition.Provides;
        }

        public bool DependsOn(DependencyNode other) => DependsOn(other, this);

        private bool DependsOn(DependencyNode other, DependencyNode from)
        {
            if (other.Instance == from.Instance)
            {
                return false;
            }

            foreach (var node in Dependencies)
            {
                if (node.Instance == from.Instance)
                {
                    throw new CyclicDependencyException(
                        $"Found cyclic dependency on node: '{node.Instance.Name}'.");
                }

                if (node.Instance == other.Instance || node.DependsOn(other, from))
                {
                    return true;
                }
            }

            return false;
        }

        public int CompareTo(DependencyNode other)
        {
            if (other.DependsOn(this))
            {
                return -1;
            }

            if (DependsOn(other))
            {
                return 1;
            }

            return 0;
        }

        public override bool Equals(object obj) => obj is DependencyNode node && node.Instance == Instance;

        public override int GetHashCode() => Instance.GetInstanceId();
    }
}
