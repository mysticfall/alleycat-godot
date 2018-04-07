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

        [Service(false)]
        protected IEnumerable<ISettingsProvider> Providers { get; private set; }

        public void AddServices(IServiceCollection collection)
        {
            var builder = CreateBuilder();

            if (Providers != null)
            {
                foreach (var provider in Providers)
                {
                    provider.AddSettings(builder);
                }
            }

            var configuration = builder.Build();

            collection
                .AddOptions()
                .AddSingleton<IConfiguration>(configuration);

            if (Providers == null) return;

            foreach (var provider in Providers)
            {
                provider.BindSettings(configuration, collection);
            }
        }

        [NotNull]
        protected virtual IConfigurationBuilder CreateBuilder()
        {
            return new ConfigurationBuilder()
                .SetFileProvider(new FileProvider())
                .SetFileLoadExceptionHandler(OnError);
        }

        protected virtual void OnError([NotNull] FileLoadExceptionContext context)
        {
        }
    }
}
