using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.Game
{
    [NonInjectable]
    public interface IDelegateNode<out T> : IGameNode where T : Node 
    {
        T Node { get; }
    }
}
