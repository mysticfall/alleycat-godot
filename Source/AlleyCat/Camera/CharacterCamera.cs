using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Locomotion;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Camera
{
    public class CharacterCamera : Godot.Camera, IOrbitable
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node]
        public ICharacter Character { get; private set; }

        [Export(PropertyHint.Range, "0, 5")]
        public float MinimumDistance { get; set; } = 0.4f;

        public Vector3 Pivot
        {
            get
            {
                if (_positionIndex == -1)
                {
                    _positionIndex = Character.Skeleton.FindBone(_headBone);
                }

                Debug.Assert(_positionIndex != -1, $"Failed to find the head bone: '{_headBone}'.");

                var head = Character.Skeleton.GetBoneGlobalPose(_positionIndex).origin;

                return Character.Skeleton.GlobalTransform.Xform(head);
            }
        }

        public Vector3 Up => Axis.Up;

        public Vector3 Forward => Character.Skeleton.GlobalTransform.basis.Forward().Project(Axis.Up);

        public Vector3 Right => Forward.Cross(Up);

        public float Pitch { get; set; }

        public float Yaw { get; set; }

        public float Distance { get; set; } = 1f;

        [Export, UsedImplicitly] private NodePath _character = "..";

        [Export, NotNull] private string _headBone = "Head";

        private int _positionIndex = -1;

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
