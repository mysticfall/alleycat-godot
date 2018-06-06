using AlleyCat.Character;
using AlleyCat.Common;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface IPerspectiveView : IView, IActivatable, IValidatable
    {
        [CanBeNull]
        IHumanoid Character { get; set; }

        bool AutoActivate { get; }
    }
}
