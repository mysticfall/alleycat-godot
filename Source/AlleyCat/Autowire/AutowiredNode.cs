using Godot;

namespace AlleyCat.Autowire
{
    public class AutowiredNode : Node
    {
        public override void _EnterTree()
        {
            base._EnterTree();

            this.Prewire();
        }

        public override void _Ready()
        {
            base._Ready();

            this.Postwire();
        }
    }
}
