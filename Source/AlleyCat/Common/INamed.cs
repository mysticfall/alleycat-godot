using AlleyCat.Autowire;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface INamed : IIdentifiable
    {
        string DisplayName { get; }
    }
}
