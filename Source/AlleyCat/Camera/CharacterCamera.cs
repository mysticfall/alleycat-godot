using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Camera
{
    public class CharacterCamera : Godot.Camera, IOrbiter
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node]
        public ICharacter Character { get; private set; }

        [Export(PropertyHint.Range, "0, 5")]
        public float MinimumDistance { get; set; } = 0.4f;

        public Vector3 Pivot => Character.Viewpoint;

        public Vector3 Up => Axis.Up;

        public Vector3 Forward => new Plane(Axis.Up, 0f).Project(Character.Skeleton.GlobalTransform.Forward());

        public Vector3 Right => Forward.Cross(Up);

        public float Pitch { get; set; }

        public float Yaw { get; set; }

        public float Distance { get; set; } = 1f;

        [Export, UsedImplicitly] private NodePath _character = "..";

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        public void Rotate(Vector2 rotation)
        {
            Yaw -= rotation.x;
            Pitch -= rotation.y;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            var pivot = Pivot;

            var direction = -Forward
                .Rotated(Up, Yaw)
                .Rotated(Right.Rotated(Up, Yaw), Pitch);

            var transform = new Transform(Basis.Identity, pivot)
                .Translated(direction * Distance)
                .LookingAt(pivot, Up);

            GlobalTransform = transform;
        }
    }
}
