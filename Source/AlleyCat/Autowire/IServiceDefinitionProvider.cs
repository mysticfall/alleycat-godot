using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IServiceDefinitionProvider
    {
        IEnumerable<Type> ProvidedTypes { get; }

        void AddServices(IServiceCollection collection);
    }
}
