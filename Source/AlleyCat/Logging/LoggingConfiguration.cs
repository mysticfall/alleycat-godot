using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlleyCat.Logging
{
    [AutowireContext]
    public class LoggingConfiguration : AutowiredNode, IServiceDefinitionProvider, IServiceFactory<ILogger>
    {
        public IEnumerable<Type> ProvidedTypes => new[]
        {
            typeof(ILoggerFactory), 
            typeof(ILogger), 
            typeof(IOptions<LoggerFilterOptions>)
        };

        [Service] private IEnumerable<ILoggerProvider> _providers;

        public void AddServices(IServiceCollection collection)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));

            collection.AddLogging(ConfigureLogger);
        }

        protected virtual void ConfigureLogger([NotNull] ILoggingBuilder builder)
        {
            Ensure.Any.IsNotNull(builder, nameof(builder));

            foreach (var provider in _providers)
            {
                builder.AddProvider(provider);
            }
        }

        public ILogger Create(IAutowireContext context, object service)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(service, nameof(service));

            var factory = this.GetRootContext().GetService<ILoggerFactory>();

            return factory?.CreateLogger(service.GetType().FullName);
        }
    }
}
