using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IAttributeProcessor
    {
        AutowirePhase ProcessPhase { get; }

        void Process(
            [NotNull] IServiceCollection collection,
            [NotNull] IServiceProvider provider, 
            [NotNull] object service);
    }
}
