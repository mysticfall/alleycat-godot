using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;

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

        public override void Process(IAutowireContext context, object service)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(service, nameof(service));

            try
            {
                Method.Invoke(service, new object[0]);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException == null)
                {
                    throw;
                }

                throw e.InnerException;
            }
        }
    }
}
