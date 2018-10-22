using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Motion
{
    public interface ITurretLike : IDirectional, IActivatable, IValidatable
    {
        float Yaw { get; set; }

        float Pitch { get; set; }

        Vector2 Rotation { get; set; }

        Range<float> YawRange { get; }

        Range<float> PitchRange { get; }

        IObservable<Vector2> OnRotationChange { get; }

        void Reset();
    }

    public static class TurretLikeExtensions
    {
        public static void Rotate(this ITurretLike turret, Vector2 rotation)
        {
            Ensure.That(turret, nameof(turret)).IsNotNull();

            turret.Rotation = rotation;
        }

        public static Basis GetBasis(this ITurretLike turret)
        {
            Ensure.That(turret, nameof(turret)).IsNotNull();

            var basis = Basis.Identity.Rotated(Vector3.Up, turret.Yaw);
            var right = basis.Xform(Vector3.Right);

            return basis.Rotated(right, turret.Pitch);
        }
    }
}
