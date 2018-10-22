using System;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public abstract class AttributeProcessor<T> : NodeProcessor where T : Attribute
    {
        public T Attribute { get; }

        protected AttributeProcessor(T attribute)
        {
            Ensure.That(attribute, nameof(attribute)).IsNotNull();

            Attribute = attribute;
        }
    }
}
