using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public abstract class OrbiterFactory<T> : TurretLikeFactory<T> where T : Orbiter
    {
        [Export(PropertyHint.Range, "0,100")]
        public float MinDistance { get; set; } = 0.1f;

        [Export(PropertyHint.Range, "0,100")]
        public float MaxDistance { get; set; } = 10f;

        [Export(PropertyHint.Range, "0,100")]
        public float InitialDistance { get; set; } = 1f;

        [Export]
        public Vector3 InitialOffset { get; set; }

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        protected override Validation<string, T> CreateService(
            Range<float> yawRange,
            Range<float> pitchRange,
            ILoggerFactory loggerFactory)
        {
            return CreateService(
                yawRange, pitchRange, new Range<float>(MinDistance, MaxDistance), loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange,
            ILoggerFactory loggerFactory);
    }
}
