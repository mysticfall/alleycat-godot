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
        public ISet<IDependencyNode> Dependencies { get; }

        public ISet<Type> Requires { get; }

        public ISet<Type> Provides { get; }

        public DependencyNode([NotNull] Node node, ServiceDefinition definition)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            Instance = node;

            Processors = definition.Processors;
            Dependencies = new HashSet<IDependencyNode>();

            Requires = definition.Requires;
            Provides = definition.Provides;
        }

        public int CompareTo(DependencyNode other)
        {
            if (other.Dependencies.Contains(this))
            {
                return -1;
            }

            if (Dependencies.Contains(other))
            {
                return 1;
            }

            return 0;
        }
    }
}
