using System;
using System.Collections.Generic;
using EnsureThat;

namespace AlleyCat.Autowire
{
    internal struct ServiceDefinition : IDependencyResolver
    {
        public Type Type { get; }

        public LanguageExt.HashSet<Type> Provides { get; }

        public LanguageExt.HashSet<Type> Requires { get; }

        public IEnumerable<INodeProcessor> Processors { get; }

        public ServiceDefinition(
            Type type, LanguageExt.HashSet<Type> provides, LanguageExt.HashSet<Type> requires,
            IEnumerable<INodeProcessor> processors)
        {
            Ensure.That(type, nameof(type)).IsNotNull();
            Ensure.That(processors, nameof(processors)).IsNotNull();

            Type = type;
            Provides = provides;
            Requires = requires;
            Processors = processors;
        }
    }
}
