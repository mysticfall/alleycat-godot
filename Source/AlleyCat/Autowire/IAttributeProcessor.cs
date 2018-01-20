using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IAttributeProcessor
    {
        AutowirePhase ProcessPhase { get; }

        void Process([NotNull] IAutowireContext context, [NotNull] object service);
    }
}
