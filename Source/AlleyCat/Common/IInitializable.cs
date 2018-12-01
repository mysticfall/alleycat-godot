using AlleyCat.Autowire;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IInitializable
    {
        void Initialize();
    }
}
