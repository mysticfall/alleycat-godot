namespace AlleyCat.Autowire
{
    public interface IServiceFactory<out T>
    {
        T Create(IAutowireContext context, object service);
    }
}
