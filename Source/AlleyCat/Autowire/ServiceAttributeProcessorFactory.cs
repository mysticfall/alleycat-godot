using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class ServiceAttributeProcessorFactory : MemberAttributeProcessorFactory<ServiceAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, ServiceAttribute attribute)
        {
            Ensure.That(member, nameof(member)).IsNotNull();
            Ensure.That(attribute, nameof(attribute)).IsNotNull();

            return new ServiceAttributeProcessor(member, attribute);
        }
    }
}
