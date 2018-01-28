using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class ServiceDefinitionProviderProcessorFactory : INodeProcessorFactory
    {
        private static readonly Type NodeType = typeof(IServiceDefinitionProvider);

        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            if (NodeType.IsAssignableFrom(type))
            {
                return new[] {new ServiceDefinitionProviderProcessor()};
            }

            return Enumerable.Empty<INodeProcessor>();
        }
    }
}
