namespace AlleyCat.Autowire
{
    public class ServiceAttribute : InjectAttribute
    {
        public ServiceAttribute(bool required = true) : base(required)
        {
        }
    }
}
