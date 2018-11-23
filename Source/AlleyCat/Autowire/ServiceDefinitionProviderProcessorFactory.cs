using System;
using System.Collections.Generic;
using System.Linq;

namespace AlleyCat.Autowire
{
    public class ServiceDefinitionProviderProcessorFactory : INodeProcessorFactory
    {
        private static readonly Type NodeType = typeof(IServiceDefinitionProvider);

        public IEnumerable<INodeProcessor> Create(Type type) =>
            NodeType.IsAssignableFrom(type)
                ? new[] {new ServiceDefinitionProviderProcessor()}
                : Enumerable.Empty<INodeProcessor>();
    }
}
