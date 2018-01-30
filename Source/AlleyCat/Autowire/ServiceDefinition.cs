using System;
using System.Collections.Generic;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    internal struct ServiceDefinition : IDependencyResolver
    {
        [NotNull]
        public Type Type { get; }

        public ISet<Type> Provides { get; }

        public ISet<Type> Requires { get; }

        [NotNull]
        public IEnumerable<INodeProcessor> Processors { get; }

        public ServiceDefinition(
            [NotNull] Type type,
            ISet<Type> provides,
            ISet<Type> requires,
            [NotNull] IEnumerable<INodeProcessor> processors)
        {
            Ensure.Any.IsNotNull(type, nameof(type));
            Ensure.Any.IsNotNull(provides, nameof(type));
            Ensure.Any.IsNotNull(requires, nameof(requires));
            Ensure.Any.IsNotNull(processors, nameof(processors));

            Type = type;
            Provides = provides;
            Requires = requires;
            Processors = processors;
        }
    }
}
