using AlleyCat.Game;
using Godot;

namespace AlleyCat.Control
{
    public abstract class InputFactory<TInput, TValue> : GameNodeFactory<TInput>
        where TInput : Input<TValue>
    {
        [Export]
        public bool Active { get; set; } = true;
    }
}
