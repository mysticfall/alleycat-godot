using AlleyCat.Common;
using Godot;

namespace AlleyCat.Locomotion
{
    public interface ILocomotion : IActivatable
    {
        Vector3 Velocity { get; }

        Vector3 RotationalVelocity { get; }

        void Move(Vector3 velocity);

        void Rotate(Vector3 velocity);
    }
}
