using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Locomotion;
using Godot;

namespace AlleyCat.Control
{
    public class CharacterControl : AutowiredNode, IActivatable
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node("..")]
        public Spatial Target { get; private set; }

        [Node]
        public InputBindings Rotation { get; private set; }

        [Node]
        public InputBindings Movement { get; private set; }

        [Node("../Locomotion")]
        public ILocomotion Locomotion { get; private set; }

        [PostConstruct]
        private void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Rotation
                .AsVector2Input()
                .Where(_ => Active)
                .Select(v => new Vector3(-v.y, -v.x, 0))
                .Subscribe(Locomotion.Rotate)
                .AddTo(this);

            Movement
                .AsVector2Input()
                .Where(_ => Active)
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Subscribe(Locomotion.Move)
                .AddTo(this);
        }
    }
}
