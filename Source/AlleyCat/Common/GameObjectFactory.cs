using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common.Generic;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public abstract class GameObjectFactory<T> : AutowiredNode, IGameObjectFactory<T>
    {
        public virtual IEnumerable<Type> ProvidedTypes => TypeUtils.FindInjectableTypes<T>();

        public Validation<string, T> Service { get; private set; } =
            Fail<string, T>("The factory has not been initialized yet.");

        Validation<string, object> IGameObjectFactory.Service => Service.Map(v => (object) v);

        [Service]
        protected Option<ILoggerFactory> LoggerFactory { get; set; }

        protected virtual Option<ILogger> Logger => LoggerFactory.Map(p => p.CreateLogger(LogCategory));

        protected virtual string LogCategory => typeof(T).FullName;

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            if (Service.IsSuccess)
            {
                throw new InvalidOperationException("The service has been already created.");
            }

            var logger = Logger.IfNone(() => new PrintLogger(LogCategory));

            (Service = CreateService(logger)).BiIter(
                service => ProvidedTypes.Iter(type => collection.AddSingleton(type, service)),
                error => logger.LogError("Failed to create a service: {}.", error));
        }

        protected abstract Validation<string, T> CreateService(ILogger logger);

        [PostConstruct]
        protected virtual void PostConstruct()
        {
            Service.SuccessAsEnumerable().OfType<IInitializable>().Iter(s => s.Initialize());
        }

        protected override void PreDestroy()
        {
            Service.SuccessAsEnumerable().OfType<IDisposable>().Iter(s => s.DisposeQuietly());
            Service = Fail<string, T>("The factory has been disposed.");

            base.PreDestroy();
        }
    }
}
