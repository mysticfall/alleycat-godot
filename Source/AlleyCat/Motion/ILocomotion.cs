using System;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.Motion
{
    public interface ILocomotion : IActivatable, IValidatable, ITimeSource
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

            return locomotion.Velocity.LengthSquared() >= Mathf.Max(threshold, 0);
        }

        public static bool IsTurning(this ILocomotion locomotion, float threshold = 0.1f)
        {
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();

            return locomotion.RotationalVelocity.LengthSquared() >= Mathf.Max(threshold, 0);
        }

        public static bool IsStationary(this ILocomotion locomotion, float threshold = 0.1f) =>
            !IsMoving(locomotion, threshold) && !IsTurning(locomotion, Mathf.Max(threshold, 0));

        public static void Stop(this ILocomotion locomotion)
        {
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();

            locomotion.Move(Vector3.Zero);
            locomotion.Rotate(Vector3.Zero);
        }
    }
}
