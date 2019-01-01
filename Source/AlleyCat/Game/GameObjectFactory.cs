using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Game.Generic;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AlleyCat.Game
{
    [NonInjectable]
    public abstract class GameObjectFactory<T> : ReactiveNode, IGameObjectFactory<T> where T : IGameObject
    {
        public virtual IEnumerable<Type> ProvidedTypes => TypeUtils.FindInjectableTypes<T>();

        public Validation<string, T> Service { get; private set; } =
            Prelude.Fail<string, T>("The factory has not been initialized yet.");

        Validation<string, object> IGameObjectFactory.Service => Service.Map(v => (object) v);

        [Service]
        protected Option<ILoggerFactory> LoggerFactory { get; set; }

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            if (Service.IsSuccess)
            {
                throw new InvalidOperationException("The service has been already created.");
            }

            var loggerFactory = LoggerFactory.IfNone(() => new NullLoggerFactory());

            (Service = CreateService(loggerFactory)).BiIter(
                service => ProvidedTypes.Iter(type => collection.AddSingleton(type, service)),
                error => throw new ValidationException(error, this));
        }

        protected abstract Validation<string, T> CreateService(ILoggerFactory loggerFactory);

        [PostConstruct]
        protected virtual void PostConstruct()
        {
            Service.SuccessAsEnumerable().OfType<IInitializable>().Iter(s => s.Initialize());
        }

        protected override void PreDestroy()
        {
            Service.SuccessAsEnumerable().OfType<IDisposable>().Iter(s => s.DisposeQuietly());
            Service = Prelude.Fail<string, T>("The factory has been disposed.");

            base.PreDestroy();
        }
    }
}
