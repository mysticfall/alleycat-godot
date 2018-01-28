using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class AutowireContext : Node, IAutowireContext
    {
        public Node Node => GetParent();

        public IAutowireContext Parent { get; private set; }

        public ISet<Type> Requires { get; } = new HashSet<Type>();

        public ISet<Type> Provides { get; } = new HashSet<Type>();

        private IServiceCollection _services;

        private ServiceProvider _provider;

        private IEnumerable<INodeProcessorFactory> _processorFactories;

        private ICollection<DependencyNode> _queue;

        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public override void _EnterTree()
        {
            base._EnterTree();

            if (GetTree().Root != Node)
            {
                Parent = Node.GetParent().GetAutowireContext();
            }

            _services = new ServiceCollection();
            _processorFactories = CreateProcessorFactories();

            _queue = new DependencyChain();
        }

        public override void _Ready()
        {
            base._Ready();

            var node = Node;

            Requires.Clear();
            Provides.Clear();

            foreach (var dependency in _queue)
            {
                Requires.UnionWith(dependency.Requires);

                if (node == dependency.Instance)
                {
                    Provides.UnionWith(dependency.Provides);
                }
            }

            Requires.ExceptWith(Provides);

            if (Parent == null)
            {
                Build();
            }
            else
            {
                Parent.Register(this);
            }
        }

        [CanBeNull]
        public object GetService([NotNull] Type serviceType)
        {
            Ensure.Any.IsNotNull(serviceType, nameof(serviceType));

            return _provider?.GetService(serviceType);
        }

        public void AddService(Action<IServiceCollection> provider)
        {
            Ensure.Any.IsNotNull(provider, nameof(provider));

            provider.Invoke(_services);

            _provider = _services.BuildServiceProvider();
        }

        public void Register(Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var definition = Cache.GetOrCreate(node.GetType(), _ => CreateDefinition(node));

            _queue.Add(new DependencyNode(node, definition));
        }

        public void Build()
        {
            foreach (var dependency in _queue)
            {
                Process(dependency);
            }

            _queue.Clear();
        }

        [NotNull]
        protected virtual IEnumerable<INodeProcessorFactory> CreateProcessorFactories()
        {
            return new List<INodeProcessorFactory>
            {
                new NodeAttributeProcessorFactory(),
                new ServiceAttributeProcessorFactory(),
                new ServiceDefinitionProviderProcessorFactory(),
                new SingletonAttributeProcessorFactory(),
                new PostConstructAttributeProcessorFactory(),
                new AutowireContextProcessorFactory()
            };
        }

        private ServiceDefinition CreateDefinition(Node node)
        {
            var processors = CreateProcessors(node);

            var provides = new HashSet<Type>();
            var requires = new HashSet<Type>();

            if (!(node is AutowireContext))
            {
                if (node is IServiceDefinitionProvider p)
                {
                    requires.UnionWith(p.ProvidedTypes);
                }

                foreach (var processor in processors)
                {
                    if (processor is IDependencyConsumer consumer)
                    {
                        requires.UnionWith(consumer.Requires);
                    }

                    if (processor is IDependencyProvider provider)
                    {
                        provides.UnionWith(provider.Provides);
                    }
                }
            }

            return new ServiceDefinition(node.GetType(), provides, requires, processors);
        }

        private IList<INodeProcessor> CreateProcessors(Node node)
        {
            var type = node.GetType();
            var processors = _processorFactories.SelectMany(f => f.Create(type)).ToList();

            processors.Sort();

            return processors;
        }

        private void Process(DependencyNode dependency, AutowirePhase? phase = null)
        {
            foreach (var processor in dependency.Processors)
            {
                if (!phase.HasValue || phase.Value == processor.ProcessPhase)
                {
                    processor.Process(this, dependency.Instance);
                }
            }
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            Parent = null;

            _services?.Clear();
            _services = null;

            _queue = null;

            _provider?.Dispose();
            _provider = null;
        }

        public override string ToString() => $"ApplicationContext({Node.Name})";
    }
}
