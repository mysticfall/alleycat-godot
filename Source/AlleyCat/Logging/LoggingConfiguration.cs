using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static LanguageExt.Prelude;

namespace AlleyCat.Logging
{
    [AutowireContext, Singleton(typeof(IServiceFactory<ILogger>))]
    public class LoggingConfiguration : AutowiredNode, IServiceDefinitionProvider, IServiceFactory<ILogger>
    {
        public const string DefaultConfigSection = "Logging";

        [Export]
        public string ConfigSection { get; set; } = DefaultConfigSection;

        public IEnumerable<Type> ProvidedTypes => Seq(
            typeof(ILoggerFactory),
            typeof(ILogger),
            typeof(IOptions<LoggerFilterOptions>));

        [Service]
        protected IEnumerable<ILoggerProvider> Providers { get; private set; } = Seq<ILoggerProvider>();

        [Service]
        protected Option<IConfiguration> Configuration { get; private set; }

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            collection.AddLogging(ConfigureLogger);
        }

        protected virtual void ConfigureLogger(ILoggingBuilder builder)
        {
            Ensure.That(builder, nameof(builder)).IsNotNull();

            var section = ConfigSection.TrimToOption().IfNone(DefaultConfigSection);

            Configuration
                .Bind(c => Optional(c.GetSection(section)))
                .Iter(c => builder.AddConfiguration(c));

            Providers.Iter(p => builder.AddProvider(p));
        }

        public ILogger Create(IAutowireContext context, object service)
        {
            Ensure.That(service, nameof(service)).IsNotNull();

            var factory = this.GetRootContext().FindService<ILoggerFactory>();

            return factory.Map(f => f.CreateLogger(service.GetType())).Head();
        }
    }
}
