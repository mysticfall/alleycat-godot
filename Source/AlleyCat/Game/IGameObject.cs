using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Game
{
    [NonInjectable]
    public interface IGameObject : IValidatable, ILifecycleAware
    {
    }
}
