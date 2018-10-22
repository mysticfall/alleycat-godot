using System.Diagnostics;
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

            var context = this.GetRootContext() as AutowireContext;

            Debug.Assert(context != null, "context != null");

            context.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            AutowireContext.Shutdown();

            base.Dispose(disposing);
        }
    }
}
