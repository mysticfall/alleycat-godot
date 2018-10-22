using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.IO;
using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Setting
{
    [AutowireContext]
    public class SettingsConfiguration : AutowiredNode, IServiceDefinitionProvider
    {
        public IEnumerable<Type> ProvidedTypes => new[] {typeof(IConfiguration)};

        protected IEnumerable<ISettingsProvider> Providers => _providers ?? Enumerable.Empty<ISettingsProvider>();

        [Service(false)] private IEnumerable<ISettingsProvider> _providers;

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            var builder = CreateBuilder();

            Providers.Iter(p => p.AddSettings(builder));

            var configuration = builder.Build();

            collection
                .AddOptions()
                .AddSingleton<IConfiguration>(configuration);

            Providers.Iter(p => p.BindSettings(configuration, collection));
        }

        protected virtual IConfigurationBuilder CreateBuilder()
        {
            return new ConfigurationBuilder()
                .SetFileProvider(new FileProvider())
                .SetFileLoadExceptionHandler(OnError);
        }

        protected virtual void OnError(FileLoadExceptionContext context)
        {
        }
    }
}
