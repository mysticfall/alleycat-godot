using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.Common
{
    [Singleton(typeof(Marker))]
    public class Marker : Spatial, IIdentifiable
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        [Export] private string _key;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
