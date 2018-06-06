using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.View;

namespace AlleyCat.Control
{
    public interface IPerspectiveView : IView, ICharacterAware<IHumanoid>, IActivatable, IValidatable
    {
        bool AutoActivate { get; }
    }
}
