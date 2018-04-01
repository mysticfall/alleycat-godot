using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public abstract class TypeAttributeProcessorFactory<T> : INodeProcessorFactory
        where T : Attribute
    {
        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            var attribute = type.GetCustomAttribute<T>();

            return attribute == null ? Enumerable.Empty<INodeProcessor>() : new[] {CreateProcessor(type, attribute)};
        }

        [NotNull]
        protected abstract INodeProcessor CreateProcessor([NotNull] Type type, [NotNull] T attribute);
    }
}
