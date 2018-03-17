using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Character
{
    [Singleton(typeof(IRaceRegistry))]
    public class RaceRegistry : Directory<IRace>, IRaceRegistry
    {
        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
