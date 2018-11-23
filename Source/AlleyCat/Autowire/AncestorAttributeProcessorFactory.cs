using System.Reflection;

namespace AlleyCat.Autowire
{
    public class AncestorAttributeProcessorFactory : MemberAttributeProcessorFactory<AncestorAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, AncestorAttribute attribute) =>
            new AncestorAttributeProcessor(member, attribute);
    }
}
