using System;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class PostConstructAttributeProcessor : AttributeProcessor<PostConstructAttribute>
    {
        [NotNull]
        public MethodInfo Method { get; }

        public override AutowirePhase ProcessPhase => AutowirePhase.PostConstruct;

        public PostConstructAttributeProcessor(
            [NotNull] MethodInfo method, [NotNull] PostConstructAttribute attribute)
            : base(attribute)
        {
            Ensure.Any.IsNotNull(method, nameof(method));

            Method = method;
        }

        public override void Process(
            IServiceCollection collection, IServiceProvider provider, object service)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));
            Ensure.Any.IsNotNull(provider, nameof(provider));
            Ensure.Any.IsNotNull(service, nameof(service));

            Method.Invoke(service, new object[0]);
        }
    }
}
