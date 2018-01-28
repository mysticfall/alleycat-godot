using System;

namespace AlleyCat.Autowire
{
    public class SingletonAttributeProcessorFactory : TypeAttributeProcessorFactory<SingletonAttribute>
    {
        protected override INodeProcessor CreateProcessor(Type type, SingletonAttribute attribute)
            => new SingletonAttributeProcessor(attribute);
    }
}
