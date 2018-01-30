using Godot;

namespace AlleyCat.Autowire
{
    public class AutowiredNode : Node
    {
        //FIXME: Workaround for godotengine/godot#15053
        private bool _reentry;

        public override void _EnterTree()
        {
            base._EnterTree();

            this.Prewire();

            _reentry = false;
        }

        public override void _Ready()
        {
            base._Ready();

            if (!_reentry)
            {
                this.Postwire();
            }

            _reentry = true;
        }
    }
}
