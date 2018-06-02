using AlleyCat.Autowire;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    [Singleton(typeof(Marker))]
    public class Marker : Spatial, IIdentifiable
    {
        public string Key => _key ?? Name;

        [Export, UsedImplicitly] private string _key;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
