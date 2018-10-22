using System.Reflection;
using System.Runtime.ExceptionServices;
using EnsureThat;
using Godot;

namespace AlleyCat.Autowire
{
    public class PostConstructAttributeProcessor : AttributeProcessor<PostConstructAttribute>
    {
        public MethodInfo Method { get; }

        public override AutowirePhase ProcessPhase =>
            Attribute.Deferred ? AutowirePhase.Deferred : AutowirePhase.PostConstruct;

        public PostConstructAttributeProcessor(
            MethodInfo method, PostConstructAttribute attribute) : base(attribute)
        {
            Ensure.That(method, nameof(method)).IsNotNull();

            Method = method;
        }

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.That(context, nameof(context)).IsNotNull();
            Ensure.That(node, nameof(node)).IsNotNull();

            try
            {
                Method.Invoke(node, new object[0]);
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
            }
        }
    }
}
