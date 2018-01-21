using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IServiceFactory<out T>
    {
        [CanBeNull]
        T Create([NotNull] IAutowireContext context, [NotNull] object service);
    }
}
