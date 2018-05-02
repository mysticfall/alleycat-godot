using AlleyCat.Character;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface IPerspectiveView : IActivatable, IValidatable
    {
        [CanBeNull]
        IHumanoid Character { get; set; }

        [CanBeNull]
        Camera Camera { get; set; }

        bool AutoActivate { get; }
    }
}
