using Godot;

namespace AlleyCat.Autowire
{
    [NonInjectable]
    public class AutowiredNode : Node
    {
        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
