using System;
using System.Reflection;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class ServiceAttributeProcessor : InjectAttributeProcessor<ServiceAttribute>
    {
        public ServiceAttributeProcessor(
            [NotNull] MemberInfo member, [NotNull] ServiceAttribute attribute) : base(member, attribute)
        {
        }

        protected override object GetDependency(IServiceProvider provider, object service) =>
            provider.GetService(TargetType);
    }
}
