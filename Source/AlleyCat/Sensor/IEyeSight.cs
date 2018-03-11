using Godot;

namespace AlleyCat.Sensor
{
    public interface IEyeSight : IVision
    {
        Transform Head { get; }
    }
}
