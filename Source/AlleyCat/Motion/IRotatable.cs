using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public interface IRotatable : IDirectional, IActivatable, IValidatable
    {
        float Yaw { get; set; }

        float Pitch { get; set; }

        Vector2 Rotation { get; set; }

        Range<float> YawRange { get; }

        Range<float> PitchRange { get; }

        IObservable<Vector2> OnRotationChange { get; }
    }

    public static class RotatableExtensions
    {
        public static void Reset([NotNull] this IRotatable rotatable)
        {
            Ensure.Any.IsNotNull(rotatable, nameof(rotatable));

            rotatable.Rotation = Vector2.Zero;
        }

        public static Basis GetBasis([NotNull] this IRotatable rotatable)
        {
            Ensure.Any.IsNotNull(rotatable, nameof(rotatable));

            var basis = Basis.Identity.Rotated(Vector3.Up, rotatable.Yaw);
            var right = basis.Xform(Vector3.Right);

            return basis.Rotated(right, rotatable.Pitch);
        }
    }
}
