using AlleyCat.Common;
using Godot;

namespace AlleyCat.Sensor
{
    public interface IVision : ISense, IDirectional
    {
        void LookAt(Vector3 target);
    }
}
