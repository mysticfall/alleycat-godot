using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public abstract class GameObjectFactory<T> : AutowiredNode, IGameObjectFactory<T>
    {
        public virtual IEnumerable<Type> ProvidedTypes
        {
            get
            {
                var type = typeof(T);

                return Cache.GetOrCreate(type, _ => FindParentTypes(type));
            }
        }

        public Validation<string, T> Service { get; private set; } =
            Fail<string, T>("The factory has not been initialized yet.");

        Validation<string, object> IGameObjectFactory.Service => Service.Map(v => (object) v);

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            if (Service.IsSuccess)
            {
                throw new InvalidOperationException("The service has been already created.");
            }

            (Service = CreateService()).BiIter(
                vision => ProvidedTypes.Iter(type => collection.AddSingleton(type, vision)),
                error => GD.Print(error) // TODO Need a better way to handle errors (i.e. using a logger)
            );
        }

        protected abstract Validation<string, T> CreateService();

        [PostConstruct]
        protected virtual void PostConstruct() => 
            Service.SuccessAsEnumerable().OfType<IInitializable>().Iter(s => s.Initialize());

        protected override void PreDestroy()
        {
            Service.SuccessAsEnumerable().OfType<IDisposable>().Iter(s => s.DisposeQuietly());
            Service = Fail<string, T>("The factory has been disposed.");

            base.PreDestroy();
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        private static IEnumerable<Type> FindParentTypes(Type type)
        {
            if (type == null || type == typeof(GameObject))
            {
                yield break;
            }

            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            var current = type.BaseType;

            while (current != null)
            {
                yield return current;

                current = current.BaseType;
            }
        }
    }
}
