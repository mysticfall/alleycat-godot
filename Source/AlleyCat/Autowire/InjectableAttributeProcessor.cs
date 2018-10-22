namespace AlleyCat.Autowire
{
    public abstract class InjectableAttributeProcessor<T> : AttributeProcessor<T>
        where T : InjectableAttribute
    {
        public override AutowirePhase ProcessPhase => AutowirePhase.Register;

        protected InjectableAttributeProcessor(T attribute) : base(attribute)
        {
        }
    }
}
