using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IServiceCollection _collection;

        private ServiceProvider _provider;

        private IEnumerable<IAttributeProcessorFactory> _processorFactories;

        private static readonly IMemoryCache InjectorCache = new MemoryCache(new MemoryCacheOptions());

        private IList<object> _queue;

        private bool _ready;

        public override void _EnterTree()
        {
            base._EnterTree();

            _collection = new ServiceCollection();
            _queue = new List<object>();

            _processorFactories = CreateProcessorFactories();
        }

        public override void _Ready()
        {
            base._Ready();

            Debug.Assert(
                _processorFactories != null, "CreateProcessorFactories() returned null.");

            _provider = _collection.BuildServiceProvider();

            foreach (var instance in _queue)
            {
                ProcessAttributes(instance, AutowirePhase.Resolve);
            }

            foreach (var instance in _queue)
            {
                ProcessAttributes(instance, AutowirePhase.PostConstruct);
            }

            _queue = null;
            _ready = true;
        }

        [CanBeNull]
        public object GetService([NotNull] Type serviceType)
        {
            Ensure.Any.IsNotNull(serviceType, nameof(serviceType));

            return _provider.GetService(serviceType);
        }

        public void Register(object instance)
        {
            Ensure.Any.IsNotNull(instance, nameof(instance));

            var provider = instance as IServiceConfiguration;

            provider?.Register(_collection);

            _queue.Add(instance);

            ProcessAttributes(instance, AutowirePhase.Register);

            if (_ready)
            {
                ProcessAttributes(instance, AutowirePhase.Resolve);
                ProcessAttributes(instance, AutowirePhase.PostConstruct);
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
                new SingletonAttributeProcessorFactory(),
                new PostConstructAttributeProcessorFactory()
            };
        }

        private void ProcessAttributes(object instance, AutowirePhase phase)
        {
            var type = instance.GetType();
            var processors = InjectorCache.GetOrCreate(type, _ => CreateProcessors(type));

            Debug.Assert(processors != null, "CreateProcessors() returned null.");

            foreach (var processor in processors)
            {
                if (processor.ProcessPhase == phase)
                {
                    processor.Process(_collection, this, instance);
                }
            }
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            _queue = null;
            _ready = false;

            _provider?.Dispose();
        }
    }
}
