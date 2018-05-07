using System;
using AlleyCat.Common;

namespace AlleyCat.Motion
{
    public interface IOrbiter : ITurretLike
    {
        float Distance { get; set; }

        Range<float> DistanceRange { get; }

        IObservable<float> OnDistanceChange { get; }
    }
}
