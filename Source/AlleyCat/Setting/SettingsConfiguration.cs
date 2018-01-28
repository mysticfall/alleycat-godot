using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.IO;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Setting
{
    [AutowireContext]
    public class SettingsConfiguration : AutowiredNode, IServiceDefinitionProvider
    {
        public IEnumerable<Type> ProvidedTypes => new[] {typeof(IConfiguration)};

        [Service(false)] private IEnumerable<ISettingsProvider> _providers;

        public void AddServices(IServiceCollection collection)
        {
            var builder = CreateBuilder();

            if (_providers != null)
            {
                foreach (var provider in _providers)
                {
                    provider.AddSettings(builder);
                }
            }

            var configuration = builder.Build();

            collection
                .AddOptions()
                .AddSingleton<IConfiguration>(configuration);
        }

        [NotNull]
        protected virtual IConfigurationBuilder CreateBuilder()
        {
            var builder = new ConfigurationBuilder();

            builder
                .SetFileProvider(new FileProvider())
                .SetFileLoadExceptionHandler(OnError);

            return builder;
        }

        protected virtual void OnError([NotNull] FileLoadExceptionContext context)
        {
        }
    }
}
