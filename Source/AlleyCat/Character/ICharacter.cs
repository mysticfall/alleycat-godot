using AlleyCat.Animation;
using AlleyCat.Motion;
using Godot;

namespace AlleyCat.Character
{
    public interface ICharacter : IRigged, ILocomotive
    {
        Vector3 Viewpoint { get; }

        Vector3 LookingAt { get; }
    }
}
