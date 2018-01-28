using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IServiceDefinitionProvider
    {
        IEnumerable<Type> ProvidedTypes { get; }

        void AddServices([NotNull] IServiceCollection collection);
    }
}
