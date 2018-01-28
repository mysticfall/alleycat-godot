using System.Reflection;
using System.Runtime.ExceptionServices;
using EnsureThat;
using Godot;
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

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(node, nameof(node));

            try
            {
                Method.Invoke(node, new object[0]);
            }
            catch (TargetInvocationException e)
            {
                //TODO: Until godotengine/godot#16107 gets fixed.
                if (e.InnerException != null)
                {
                    GD.Print(e.InnerException.Message);
                    GD.Print(e.InnerException.StackTrace);
                }

                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
            }
        }
    }
}
