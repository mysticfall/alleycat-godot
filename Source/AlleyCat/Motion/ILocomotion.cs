using AlleyCat.Common;
using Godot;

namespace AlleyCat.Motion
{
    public interface ILocomotion : IActivatable, IValidatable
    {
        Vector3 Velocity { get; }

        Vector3 RotationalVelocity { get; }

        void Move(Vector3 velocity);

        void Rotate(Vector3 velocity);
    }
}
