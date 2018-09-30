namespace AlleyCat.Autowire
{
    public class ServiceAttribute : InjectAttribute
    {
        public bool IncludeInherited { get; }

        public ServiceAttribute(bool required = true, bool includeInherited = true) : base(required)
        {
            IncludeInherited = includeInherited;
        }
    }
}
