using Microsoft.Extensions.Configuration;

namespace AlleyCat.Setting
{
    public interface IWritableConfigurationProvider : IConfigurationProvider
    {
        void Save();
    }
}
