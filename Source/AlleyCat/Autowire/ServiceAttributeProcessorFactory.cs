using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class ServiceAttributeProcessorFactory : MemberAttributeProcessorFactory<ServiceAttribute>
    {
        protected override IAttributeProcessor CreateProcessor(
            MemberInfo member, ServiceAttribute attribute)
        {
            Ensure.Any.IsNotNull(member, nameof(member));
            Ensure.Any.IsNotNull(attribute, nameof(attribute));

            return new ServiceAttributeProcessor(member, attribute);
        }
    }
}
