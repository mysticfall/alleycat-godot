using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace AlleyCat.Setting
{
    public interface ISettingsProvider
    {
        void AddSettings([NotNull] IConfigurationBuilder builder);
    }
}
