using System;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Motion
{
    public interface IRotatable: IDirectional, IActivatable, IValidatable
    {
        float Yaw { get; set; }

        float Pitch { get; set; }

        Vector2 Rotation { get; set; }

        Range<float> YawRange { get; }

        Range<float> PitchRange { get; }

        IObservable<Vector2> OnRotationChange { get; }
    }
}
