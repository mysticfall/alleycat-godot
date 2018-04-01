using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    internal struct DependencyNode : IDependencyResolver, IComparable<DependencyNode>
    {
        [NotNull]
        public Node Instance { get; }

        [NotNull]
        public ISet<DependencyNode> Dependencies { get; }

        public ISet<Type> Requires => _resolver.Requires;

        public ISet<Type> Provides => _resolver.Provides;

        private readonly Action<IAutowireContext> _processor;

        private readonly IDependencyResolver _resolver;

        public DependencyNode([NotNull] AutowireContext context)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            Instance = context.Node;
            Dependencies = new HashSet<DependencyNode>();

            _resolver = context;
            _processor = _ => context.Build();
        }

        public DependencyNode([NotNull] Node node, ServiceDefinition definition)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            Instance = node;
            Dependencies = new HashSet<DependencyNode>();

            _resolver = definition;

            _processor = context =>
            {
                foreach (var processor in definition.Processors)
                {
                    processor.Process(context, node);
                }
            };
        }

        public void Process(IAutowireContext context) => _processor.Invoke(context);

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

            return DependsOn(other) ? 1 : 0;
        }

        public override bool Equals(object obj) => obj is DependencyNode node && node.Instance == Instance;

        public override int GetHashCode() => Instance.GetInstanceId();
    }
}
