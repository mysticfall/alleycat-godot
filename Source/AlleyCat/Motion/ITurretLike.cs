using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

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
        public static void Rotate([NotNull] this ITurretLike turret, Vector2 rotation)
        {
            Ensure.Any.IsNotNull(turret, nameof(turret));

            turret.Rotation = rotation;
        }

        public static Basis GetBasis([NotNull] this ITurretLike rotatable)
        {
            Ensure.Any.IsNotNull(rotatable, nameof(rotatable));

            var basis = Basis.Identity.Rotated(Vector3.Up, rotatable.Yaw);
            var right = basis.Xform(Vector3.Right);

            return basis.Rotated(right, rotatable.Pitch);
        }
    }
}
