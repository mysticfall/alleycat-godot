using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Setting
{
    public interface ISettingsProvider
    {
        void AddSettings([NotNull] IConfigurationBuilder builder);

        void BindSettings([NotNull] IConfigurationRoot root, [NotNull] IServiceCollection collection);
    }
}
