using Godot;

namespace AlleyCat.Character
{
    public interface IHumanoid : ICharacter
    {
        Transform Head { get; }
    }
}
