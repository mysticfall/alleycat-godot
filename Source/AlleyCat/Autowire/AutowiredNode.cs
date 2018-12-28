using AlleyCat.Common;

namespace AlleyCat.Autowire
{
    [NonInjectable]
    public class AutowiredNode : BaseNode
    {
        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
