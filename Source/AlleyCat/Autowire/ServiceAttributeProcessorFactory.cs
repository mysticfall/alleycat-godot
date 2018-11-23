using System.Reflection;

namespace AlleyCat.Autowire
{
    public class ServiceAttributeProcessorFactory : MemberAttributeProcessorFactory<ServiceAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, ServiceAttribute attribute) =>
            new ServiceAttributeProcessor(member, attribute);
    }
}
