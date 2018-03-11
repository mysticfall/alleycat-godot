using Godot;

namespace AlleyCat.Sensor
{
    public interface IPairedEyeSight : IEyeSight
    {
        Transform LeftEye { get; }

        Transform RightEye { get; }
    }
}
