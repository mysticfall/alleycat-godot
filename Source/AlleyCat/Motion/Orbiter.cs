using System;
using Godot;

namespace AlleyCat.Motion
{
    public abstract class Orbiter : Node, IOrbiter
    {
        public abstract Spatial Target { get; }

        public abstract Vector3 Pivot { get; }

        public abstract Vector3 Up { get; }

        public abstract Vector3 Forward { get; }

        public Vector3 Right => Forward.Cross(Up);

        public float Pitch { get; set; }

        public float Yaw { get; set; }

        public float Distance { get; set; } = 1f;

        public void Rotate(Vector2 rotation)
        {
            Yaw -= rotation.x;
            Pitch -= rotation.y;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            var pivot = Pivot;

            var yaw = Math.PI / 180 * NormalizeAspectAngle(Yaw);
            var pitch = Math.PI / 180 * NormalizeAspectAngle(Pitch);

            var direction = -Forward
                .Rotated(Up, (float) yaw)
                .Rotated(Right.Rotated(Up, (float) yaw), (float) pitch);

            var transform = new Transform(Basis.Identity, pivot)
                .Translated(direction * Distance)
                .LookingAt(pivot, Up);

            Target.GlobalTransform = transform;
        }

        public static float NormalizeAspectAngle(float degrees)
        {
            var value = degrees;

            while (value < 0) value += 360;

            return value > 180 ? value - 360 : value;
        }
    }
}
