using Godot;

namespace AlleyCat.Autowire
{
    public class AutowiredNode : Node
    {
        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
