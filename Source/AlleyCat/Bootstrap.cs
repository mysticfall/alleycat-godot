using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;

namespace AlleyCat
{
    public class Bootstrap : Node
    {
        public Bootstrap()
        {
            VariantTypeConverter.Install();
        }

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
