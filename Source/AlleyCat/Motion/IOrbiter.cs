using System;
using AlleyCat.Common;

namespace AlleyCat.Motion
{
    public interface IOrbiter : IRotatable
    {
        float Distance { get; set; }

        Range<float> DistanceRange { get; }

        IObservable<float> OnDistanceChange { get; }
    }
}
