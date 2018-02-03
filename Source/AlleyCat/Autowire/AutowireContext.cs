using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class AutowireContext : IAutowireContext
    {
        public Node Node { get; }

        public IAutowireContext Parent =>
            Node.GetTree().Root != Node ? Node.GetParent().GetAutowireContext() : null;

        public ISet<Type> Requires { get; } = new HashSet<Type>();

        public ISet<Type> Provides { get; } = new HashSet<Type>();

        [NotNull] public static ICollection<INodeProcessorFactory> ProcessorFactories =
            new List<INodeProcessorFactory>
            {
                new NodeAttributeProcessorFactory(),
                new ServiceAttributeProcessorFactory(),
                new ServiceDefinitionProviderProcessorFactory(),
                new SingletonAttributeProcessorFactory(),
                new PostConstructAttributeProcessorFactory()
            };

        private IServiceCollection _services;

        private ServiceProvider _provider;

        private ICollection<DependencyNode> _queue;

        private bool _built;

        private bool _disposed;

        private static readonly NodeStore<AutowireContext> Store = new NodeStore<AutowireContext>();

        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        private AutowireContext([NotNull] Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            Node = node;

            _services = new ServiceCollection();
            _queue = new DependencyChain();
        }

        [CanBeNull]
        public object GetService([NotNull] Type serviceType)
        {
            CheckDisposed();

            Ensure.Any.IsNotNull(serviceType, nameof(serviceType));

            return _provider?.GetService(serviceType);
        }

        public void AddService([NotNull] Action<IServiceCollection> provider)
        {
            CheckDisposed();

            Ensure.Any.IsNotNull(provider, nameof(provider));

            provider.Invoke(_services);

            _provider = _services.BuildServiceProvider();
        }

        internal void Register([NotNull] Node node)
        {
            if (node == node.GetTree().Root)
            {
                return;
            }

            var definition = Cache.GetOrCreate(node.GetType(), _ => CreateDefinition(node));

            Register(new DependencyNode(node, definition));
        }

        internal void Register([NotNull] AutowireContext context) => Register(new DependencyNode(context));

        private void Register(DependencyNode node)
        {
            CheckDisposed();

            if (_built)
            {
                node.Process(this);
            }
            else
            {
                _queue.Add(node);
            }
        }

        internal void Initialize()
        {
            CheckDisposed();

            Register(Node);

            var node = Node;

            Requires.Clear();
            Provides.Clear();

            var localProvides = new HashSet<Type>();

            foreach (var dependency in _queue)
            {
                Requires.UnionWith(dependency.Requires);

                if (node == dependency.Instance)
                {
                    Provides.UnionWith(dependency.Provides);
                }

                localProvides.UnionWith(dependency.Provides);
            }

            Requires.ExceptWith(localProvides);

            if (Parent == null)
            {
                Build();
            }
            else
            {
                (Parent as AutowireContext)?.Register(this);
            }
        }

        internal void Build()
        {
            CheckDisposed();

            _built = true;

            foreach (var dependency in _queue)
            {
                dependency.Process(this);
            }

            _queue.Clear();
        }

        private ServiceDefinition CreateDefinition(Node node)
        {
            var processors = CreateProcessors(node);

            var provides = new HashSet<Type>();
            var requires = new HashSet<Type>();

            if (node is IServiceDefinitionProvider p)
            {
                provides.UnionWith(p.ProvidedTypes);
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

            return new ServiceDefinition(node.GetType(), provides, requires, processors);
        }

        private IList<INodeProcessor> CreateProcessors(Node node)
        {
            var type = node.GetType();
            var processors = ProcessorFactories.SelectMany(f => f.Create(type)).ToList();

            processors.Sort();

            return processors;
        }

        public void Dispose()
        {
            CheckDisposed();

            _services?.Clear();
            _services = null;

            _queue = null;

            _provider?.Dispose();
            _provider = null;

            _disposed = true;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new InvalidOperationException($"{this} has been disposed already.");
            }
        }

        public override string ToString() => $"ApplicationContext({Node.Name})";

        internal static AutowireContext GetOrCreate(Node node) => 
            Store.GetOrCreate(node, _ => new AutowireContext(node));

        internal static void Shutdown() => Store.Dispose();
    }
}
