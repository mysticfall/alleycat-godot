using System;
using System.Collections.Generic;
using System.Reflection;
using EnsureThat;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public abstract class TypeAttributeProcessorFactory<T> : INodeProcessorFactory
        where T : System.Attribute
    {
        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.That(type, nameof(type)).IsNotNull();

            return Optional(type.GetCustomAttribute<T>()).AsEnumerable().Map(a => CreateProcessor(type, a));
        }

        protected abstract INodeProcessor CreateProcessor(Type type, T attribute);
    }
}
