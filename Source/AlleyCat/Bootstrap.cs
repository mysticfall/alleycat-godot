using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat
{
    public class Bootstrap : Node
    {
        public Bootstrap()
        {
            Ensure.Off();

            ToggleAssert();

            VariantTypeConverter.Install();
        }

        [Conditional("DEBUG")]
        private static void ToggleAssert() => Ensure.On();

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
