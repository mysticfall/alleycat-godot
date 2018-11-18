using AlleyCat.Common;
using Godot;

namespace AlleyCat.Control
{
    public abstract class InputFactory<TInput, TValue> : GameObjectFactory<TInput>
        where TInput : Input<TValue>
    {
        [Export]
        public bool Active { get; set; } = true;
    }
}
