using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IServiceConfiguration
    {
        void Register([NotNull] IServiceCollection collection);
    }
}
