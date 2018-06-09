using AlleyCat.IO;
using Godot;

namespace AlleyCat.Game
{
    public interface IScene : IStateHolder
    {
        PackedScene Pack();
    }
}
