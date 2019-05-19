using Godot;

namespace AlleyCat.Game
{
    public interface IDelegateObject<out T> : IGameObject where T : Node 
    {
        T Node { get; }
    }
}
