using System.Reflection;
using EnsureThat;
using Godot;

namespace AlleyCat.Autowire
{
    public class AncestorAttributeProcessorFactory : MemberAttributeProcessorFactory<AncestorAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, AncestorAttribute attribute)
        {
            Ensure.Any.IsNotNull(member, nameof(member));
            Ensure.Any.IsNotNull(attribute, nameof(attribute));

            return new AncestorAttributeProcessor(member, attribute);
        }
    }
}
