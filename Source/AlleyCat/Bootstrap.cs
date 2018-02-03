using AlleyCat.Autowire;
using Godot;

namespace AlleyCat
{
    public class Bootstrap : Node
    {
        public override void _Ready()
        {
            base._Ready();

            (this.GetRootContext() as AutowireContext)?.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            AutowireContext.Shutdown();

            base.Dispose(disposing);
        }
    }
}
