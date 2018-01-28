using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class NodeAttributeProcessorFactory : MemberAttributeProcessorFactory<NodeAttribute>
    {
        protected override INodeProcessor CreateProcessor(
            MemberInfo member, NodeAttribute attribute)
        {
            Ensure.Any.IsNotNull(member, nameof(member));
            Ensure.Any.IsNotNull(attribute, nameof(attribute));

            return new NodeAttributeProcessor(member, attribute);
        }
    }
}
