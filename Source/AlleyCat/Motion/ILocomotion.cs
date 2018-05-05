using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public interface ILocomotion : IActivatable, IValidatable
    {
        Vector3 Velocity { get; }

        Vector3 RotationalVelocity { get; }

        IObservable<Vector3> OnVelocityChange { get; }

        IObservable<Vector3> OnRotationalVelocityChange { get; }

        void Move(Vector3 velocity);

        void Rotate(Vector3 velocity);
    }

    public static class LocomotionExtensions
    {
        public static bool IsMoving([NotNull] this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.Any.IsNotNull(locomotion, nameof(locomotion));

            return locomotion.Velocity.LengthSquared() >= threshold;
        }

        public static bool IsTurning([NotNull] this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.Any.IsNotNull(locomotion, nameof(locomotion));

            return locomotion.RotationalVelocity.LengthSquared() >= threshold;
        }

        public static bool IsStationary([NotNull] this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.Any.IsNotNull(locomotion, nameof(locomotion));

            return !IsMoving(locomotion, threshold) && !IsTurning(locomotion, threshold);
        }

        public static void Stop([NotNull] this ILocomotion locomotion)
        {
            Ensure.Any.IsNotNull(locomotion, nameof(locomotion));

            locomotion.Move(Vector3.Zero);
            locomotion.Rotate(Vector3.Zero);
        }
    }
}
