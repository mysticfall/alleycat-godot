using System;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class SingletonAttributeProcessor : InjectableAttributeProcessor<SingletonAttribute>
    {
        public SingletonAttributeProcessor([NotNull] SingletonAttribute attribute) : base(attribute)
        {
        }

        public override void Process(
            IServiceCollection collection, IServiceProvider provider, object service)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));
            Ensure.Any.IsNotNull(provider, nameof(provider));
            Ensure.Any.IsNotNull(service, nameof(service));

            foreach (var type in Attribute.Types)
            {
                collection.AddSingleton(type, service);
            }
        }
    }
}
