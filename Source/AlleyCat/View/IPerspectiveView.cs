using AlleyCat.Character;
using AlleyCat.Common;

namespace AlleyCat.View
{
    public interface IPerspectiveView : IView, ICharacterAware<IHumanoid>, IActivatable, IValidatable
    {
        bool AutoActivate { get; }
    }
}
