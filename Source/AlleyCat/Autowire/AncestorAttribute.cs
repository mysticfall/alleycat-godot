namespace AlleyCat.Autowire
{
    public class AncestorAttribute : InjectAttribute
    {
        public AncestorAttribute(bool required = false) : base(required)
        {
        }
    }
}
