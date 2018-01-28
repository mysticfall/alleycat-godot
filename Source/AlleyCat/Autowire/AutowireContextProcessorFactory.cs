using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class AutowireContextProcessorFactory : INodeProcessorFactory
    {
        private static readonly Type NodeType = typeof(IAutowireContext);

        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            if (NodeType.IsAssignableFrom(type))
            {
                return new[] {new AutowireContextProcessor()};
            }

            return Enumerable.Empty<INodeProcessor>();
        }
    }
}
