using System;
using AlleyCat.Autowire;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Motion
{
    public abstract class Orbiter : AutowiredNode, IOrbiter
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node]
        public Spatial Target { get; private set; }

        [Export(PropertyHint.Range, "0, 5")]
        public float MinimumDistance { get; set; } = 0.4f;

        public abstract Vector3 Origin { get; }

        public abstract Vector3 Up { get; }

        public abstract Vector3 Forward { get; }

        public Vector3 Right => Forward.Cross(Up);

        public float Pitch
        {
            get => _pitch;
            set => _pitch = Mathf.Clamp(NormalizeAspectAngle(value), MinimumPitch, MaximumPitch);
        }

        public float Yaw
        {
            get => _yaw;
            set => _yaw = Mathf.Clamp(NormalizeAspectAngle(value), MinimumYaw, MaximumYaw);
        }

        public float Distance
        {
            get => _distance;
            set => _distance = Math.Max(value, MinimumDistance);
        }

        protected virtual Transform TargetTransform
        {
            get
            {
                var pivot = Origin;

                var direction = -Forward
                    .Rotated(Up, Yaw)
                    .Rotated(Right.Rotated(Up, Yaw), Pitch);

                return new Transform(Basis.Identity, pivot)
                    .Translated(direction * Distance)
                    .LookingAt(pivot, Up);
            }
        }

        [Export, UsedImplicitly] private NodePath _target = "..";

        public virtual float MaximumPitch => Mathf.PI / 2f;

        public virtual float MinimumPitch => -Mathf.PI / 2f;

        public virtual float MaximumYaw => Mathf.PI;

        public virtual float MinimumYaw => -Mathf.PI;

        private float _pitch;

        private float _yaw;

        private float _distance = 1f;

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

            if (!Active) return;

            Target.GlobalTransform = TargetTransform;
        }

        private static float NormalizeAspectAngle(float angle)
        {
            var value = angle;

            while (value < 0) value += 2 * Mathf.PI;

            return value > Mathf.PI ? value - 2 * Mathf.PI : value;
        }
    }
}
