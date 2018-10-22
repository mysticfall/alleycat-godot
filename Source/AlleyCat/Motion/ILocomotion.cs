using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Motion
{
    public interface ILocomotion : IActivatable, IValidatable, IGameLoopAware
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
        public static bool IsMoving(this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();

            return locomotion.Velocity.LengthSquared() >= Mathf.Min(threshold, 0);
        }

        public static bool IsTurning(this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();

            return locomotion.RotationalVelocity.LengthSquared() >= Mathf.Min(threshold, 0);
        }

        public static bool IsStationary(this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();

            return !IsMoving(locomotion, threshold) && !IsTurning(locomotion, Mathf.Min(threshold, 0));
        }

        public static void Stop(this ILocomotion locomotion)
        {
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();

            locomotion.Move(Vector3.Zero);
            locomotion.Rotate(Vector3.Zero);
        }
    }
}
