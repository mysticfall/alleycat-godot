using AlleyCat.Motion;
using Godot;

namespace AlleyCat.Sensor
{
    public interface IVision : ISense, IRotatable
    {
        void LookAt(Vector3 target);
    }
}
