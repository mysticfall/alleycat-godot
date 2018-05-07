using AlleyCat.Motion;
using Godot;

namespace AlleyCat.Sensor
{
    public interface IVision : ISense, ITurretLike
    {
        Vector3 Viewpoint { get; }

        Vector3 LookDirection { get; }

        void LookAt(Vector3 target);
    }
}
