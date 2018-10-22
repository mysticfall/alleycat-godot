using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static LanguageExt.Prelude;

namespace AlleyCat.Logging
{
    [AutowireContext]
    public class LoggingConfiguration : AutowiredNode, IServiceDefinitionProvider, IServiceFactory<ILogger>
    {
        public IEnumerable<Type> ProvidedTypes => Seq(
            typeof(ILoggerFactory),
            typeof(ILogger),
            typeof(IOptions<LoggerFilterOptions>));

        [Service] private IEnumerable<ILoggerProvider> _providers = Seq<ILoggerProvider>();

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            collection.AddLogging(ConfigureLogger);
        }

        protected virtual void ConfigureLogger(ILoggingBuilder builder)
        {
            Ensure.That(builder, nameof(builder)).IsNotNull();

            _providers.Iter(p => builder.AddProvider(p));
        }

        public ILogger Create(IAutowireContext context, object service)
        {
            Ensure.That(context, nameof(context)).IsNotNull();
            Ensure.That(service, nameof(service)).IsNotNull();

            var factory = this.GetRootContext().FindService<ILoggerFactory>();

            return factory.Map(f => f.CreateLogger(service.GetType().FullName)).Head();
        }
    }
}
