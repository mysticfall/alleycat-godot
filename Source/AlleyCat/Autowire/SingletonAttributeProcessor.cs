using System.Diagnostics;
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

        public override void Process(IAutowireContext context, object service)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(service, nameof(service));

            var target = context.Node == service ? context.Parent : context;

            Debug.Assert(target != null, "context.Parent != null");

            foreach (var type in Attribute.Types)
            {
                target.ServiceCollection.AddSingleton(type, service);
            }
        }
    }
}
