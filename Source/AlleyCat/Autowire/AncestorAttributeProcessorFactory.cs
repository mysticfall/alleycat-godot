using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class AncestorAttributeProcessorFactory : MemberAttributeProcessorFactory<AncestorAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, AncestorAttribute attribute)
        {
            Ensure.That(member, nameof(member)).IsNotNull();
            Ensure.That(attribute, nameof(attribute)).IsNotNull();

            return new AncestorAttributeProcessor(member, attribute);
        }
    }
}
