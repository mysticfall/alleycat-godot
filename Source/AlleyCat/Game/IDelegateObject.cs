using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.Game
{
    [NonInjectable]
    public interface IDelegateObject<out T> : IGameObject where T : Node 
    {
        T Node { get; }
    }
}
