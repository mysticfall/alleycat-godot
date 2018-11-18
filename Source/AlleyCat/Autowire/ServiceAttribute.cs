namespace AlleyCat.Autowire
{
    public class ServiceAttribute : InjectAttribute
    {
        public bool Local { get; }

        public ServiceAttribute(bool required = false, bool local = false) : base(required)
        {
            Local = local;
        }
    }
}
