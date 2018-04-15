using System;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Motion
{
    public interface IOrbiter : IDirectional, IActivatable, IValidatable
    {
        float Yaw { get; set; }

        float Pitch { get; set; }

        float Distance { get; set; }

        Vector2 Rotation { get; set; }

        Range<float> YawRange { get; }

        Range<float> PitchRange { get; }

        Range<float> DistanceRange { get; }

        IObservable<Vector2> OnRotationChange { get; }

        IObservable<float> OnDistanceChange { get; }
    }
}
