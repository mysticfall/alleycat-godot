using AlleyCat.Character;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    public interface IPerspectiveView : IView, ICharacterAware<IHumanoid>, IActivatable, IValidatable
    {
        bool AutoActivate { get; }
    }
}
