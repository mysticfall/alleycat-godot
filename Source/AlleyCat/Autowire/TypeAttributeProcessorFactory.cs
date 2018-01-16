using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public abstract class TypeAttributeProcessorFactory<T> : IAttributeProcessorFactory
        where T : Attribute
    {
        public IEnumerable<IAttributeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            var attribute = type.GetCustomAttribute<T>();

            if (attribute == null)
            {
                return Enumerable.Empty<IAttributeProcessor>();
            }

            return new[] {CreateProcessor(type, attribute)};
        }

        [NotNull]
        protected abstract IAttributeProcessor CreateProcessor(
            [NotNull] Type type, [NotNull] T attribute);
    }
}
