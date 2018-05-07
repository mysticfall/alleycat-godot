using AlleyCat.Motion;
using Godot;

namespace AlleyCat.Sensor
{
    public interface IVision : ISense, ITurretLike
    {
        void LookAt(Vector3 target);
    }
}
