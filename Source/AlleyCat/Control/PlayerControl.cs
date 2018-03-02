using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Camera;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Control
{
    public class PlayerControl : AutowiredNode, IActivatable
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node("..")]
        public ICharacter Character { get; private set; }

        [Node("../Player Camera")]
        public CharacterCamera Camera { get; private set; }

        [Node]
        public InputBindings Movement { get; private set; }

        [Node]
        public InputBindings Rotation { get; private set; }

        [Node]
        public InputBindings Zoom { get; private set; }

        [PostConstruct]
        private void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Movement
                .AsVector2Input()
                .Where(_ => Active && Character?.Locomotion != null)
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Subscribe(v => Character.Locomotion.Move(v))
                .AddTo(this);

            Rotation
                .AsVector2Input()
                .Where(_ => Active)
                .Select(v => v * 0.05f)
                .Subscribe(Camera.Rotate)
                .AddTo(this);

            Zoom
                .GetAxis("Value")
                .Where(_ => Active)
                .Select(v => Math.Max(Camera.Distance - v * 0.05f, Camera.MinimumDistance))
                .Subscribe(v => Camera.Distance = v)
                .AddTo(this);

            this.OnProcess()
                .Subscribe(Rotate)
                .AddTo(this);
        }

        private void Rotate(float delta)
        {
            if (Character.Locomotion.Velocity.LengthSquared() < 0.1f)
            {
                Character.Locomotion.Rotate(Axis.Zero);

                return;
            }

            var offset = Mathf.Lerp(0, Camera.Yaw, delta * 1.5f);

            Character.Locomotion.Rotate(Axis.Up * offset);
            Camera.Yaw -= offset;
        }
    }
}
