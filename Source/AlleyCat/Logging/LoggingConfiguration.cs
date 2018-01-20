using System;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [AutowireContext, UsedImplicitly]
    public class LoggingConfiguration : Node, IServiceConfiguration
    {
        public void Register(IServiceCollection collection)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));

            var factory = new LoggerFactory();
            var providers = this.GetChildren<ILoggerProvider>();
            var registered = false;

            foreach (var provider in providers)
            {
                GD.Print($"Logging: Registering logger provider: '{provider.GetType().FullName}'.");

                factory.AddProvider(provider);

                if (!registered) registered = true;
            }

            if (!registered)
            {
                GD.Print("Warning: No logging provider has been configured.");
            }

            collection.AddTransient(_ => CreateLoggerForInvoker(factory));
            collection.AddSingleton<ILoggerFactory>(factory);
        }

        private static ILogger CreateLoggerForInvoker(ILoggerFactory factory)
        {
            var stack = new StackTrace(false);
            var markerType = typeof(AutowireContext);

            var type = stack.GetFrames()?
                .Select(f => f.GetMethod().DeclaringType)
                .SkipWhile(t => t != markerType)
                .SkipWhile(t => t == markerType)
                .First();

            if (type == null)
            {
                throw new InvalidOperationException("Failed to determine invoker class.");
            }

            return factory.CreateLogger(type);
        }

        public override void _EnterTree() => this.Prewire();

        public override void _Ready() => this.Postwire();
    }
}
