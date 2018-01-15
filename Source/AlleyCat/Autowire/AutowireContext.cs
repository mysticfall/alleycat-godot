using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class AutowireContext : Node, IAutowireContext
    {
        public const string RootContextName = "RootContext";

        private IServiceCollection _collection;

        private ServiceProvider _provider;

        private IEnumerable<IAttributeProcessorFactory> _processorFactories;

        private static readonly IMemoryCache InjectorCache = new MemoryCache(new MemoryCacheOptions());

        public override void _Ready()
        {
            base._Ready();

            _processorFactories = CreateProcessorFactories();

            Debug.Assert(
                _processorFactories != null, "CreateProcessorFactories() returned null.");

            _collection = new ServiceCollection();

            var configs = this.GetChildren<IServiceConfiguration>();

            foreach (var config in configs)
            {
                config.Register(_collection);
            }

            _provider = _collection.BuildServiceProvider();
        }

        [CanBeNull]
        public object GetService([NotNull] Type serviceType)
        {
            Ensure.Any.IsNotNull(serviceType, nameof(serviceType));

            return _provider.GetService(serviceType);
        }

        public void Resolve(object instance)
        {
            Ensure.Any.IsNotNull(instance, nameof(instance));

            var type = instance.GetType();
            var processors = InjectorCache.GetOrCreate(type, _ => CreateProcessors(type));

            Debug.Assert(processors != null, "CreateProcessors() returned null.");

            foreach (var processor in processors)
            {
                processor.Process(this, instance);
            }
        }

        [NotNull]
        protected IEnumerable<IAttributeProcessor> CreateProcessors([NotNull] Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            return _processorFactories.SelectMany(f => f.Create(type));
        }

        [NotNull]
        protected virtual IEnumerable<IAttributeProcessorFactory> CreateProcessorFactories()
        {
            return new List<IAttributeProcessorFactory>
            {
                new NodeAttributeProcessorFactory(),
                new ServiceAttributeProcessorFactory(),
            };
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            _provider?.Dispose();
        }

        public static AutowireContext CreateRootContext() => new AutowireContext {Name = RootContextName};
    }
}
