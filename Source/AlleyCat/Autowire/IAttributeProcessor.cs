using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IAttributeProcessor
    {
        void Process([NotNull] IAutowireContext context, [NotNull] object service);
    }
}
