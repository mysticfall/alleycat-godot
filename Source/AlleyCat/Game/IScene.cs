using Godot;

namespace AlleyCat.Game
{
    public interface IScene
    {
        PackedScene Pack();

        NodePath CharactersPath { get; }

        NodePath ItemsPath { get; }
    }
}
