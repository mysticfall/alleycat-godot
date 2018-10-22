using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Setting
{
    public interface ISettingsProvider
    {
        void AddSettings(IConfigurationBuilder builder);

        void BindSettings(IConfigurationRoot root, IServiceCollection collection);
    }
}
