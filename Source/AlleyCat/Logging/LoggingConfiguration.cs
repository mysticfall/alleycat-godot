using System.Collections.Generic;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [AutowireContext]
    public class LoggingConfiguration : AutowiredNode, IServiceConfiguration, IServiceFactory<ILogger>
    {
        public ILoggerFactory LoggerFactory { get; private set; }

        [Service] private IEnumerable<ILoggerProvider> _providers;

        public void Register(IServiceCollection collection)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));

            LoggerFactory = new LoggerFactory();

            collection
                .AddSingleton(LoggerFactory)
                .AddSingleton<IServiceFactory<ILogger>>(this);
        }

        [PostConstruct]
        private void OnInitialize()
        {
            var registered = false;

            foreach (var provider in _providers)
            {
                GD.Print($"Logging: Registering logger provider: '{provider.GetType().FullName}'.");

                LoggerFactory.AddProvider(provider);

                if (!registered) registered = true;
            }

            if (!registered)
            {
                GD.Print("Warning: No logging provider has been configured.");
            }
        }

        public ILogger Create(IAutowireContext context, object service)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(service, nameof(service));

            return LoggerFactory?.CreateLogger(service.GetType().FullName);
        }
    }
}
