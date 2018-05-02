using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface IViewControl : IControl
    {
        [CanBeNull]
        Camera Camera { get; set; }
    }
}
