using EnsureThat;

namespace AlleyCat.Autowire
{
    public abstract class AttributeProcessor<T> : NodeProcessor where T : System.Attribute
    {
        public T Attribute { get; }

        protected AttributeProcessor(T attribute)
        {
            Ensure.That(attribute, nameof(attribute)).IsNotNull();

            Attribute = attribute;
        }

        public override string ToString() => Attribute.ToString();
    }
}
