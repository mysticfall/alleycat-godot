using System.Collections;
using System.Reflection;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Autowire
{
    public class AncestorAttributeProcessor : InjectAttributeProcessor<AncestorAttribute>
    {
        public AncestorAttributeProcessor(MemberInfo member, AncestorAttribute attribute)
            : base(member, attribute)
        {
        }

        protected override IEnumerable GetDependencies(IAutowireContext context, Node node)
        {
            Ensure.That(context, nameof(context)).IsNotNull();
            Ensure.That(node, nameof(node)).IsNotNull();

            return EnumerableHelper.OfType(node.GetAncestors(), DependencyType);
        }
    }
}
