using System;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public abstract class AttributeProcessor<T> : NodeProcessor where T : Attribute
    {
        [NotNull]
        public T Attribute { get; }

        protected AttributeProcessor([NotNull] T attribute)
        {
            Ensure.Any.IsNotNull(attribute, nameof(attribute));

            Attribute = attribute;
        }
    }
}
