using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Logging;
using static Godot.Mathf;

namespace AlleyCat.Motion
{
    public abstract class TurretLikeFactory<T> : GameNodeFactory<T> where T : TurretLike
    {
        [Export]
        public bool Active { get; set; } = true;

        [Export(PropertyHint.Range, "-180,180")]
        public float MaxYaw { get; set; } = 180f;

        [Export(PropertyHint.Range, "-180,180")]
        public float MinYaw { get; set; } = -180f;

        [Export(PropertyHint.Range, "-90,90")]
        public float MaxPitch { get; set; } = 90f;

        [Export(PropertyHint.Range, "-90,90")]
        public float MinPitch { get; set; } = -90f;

        protected override Validation<string, T> CreateService(ILoggerFactory loggerFactory) =>
            CreateService(
                new Range<float>(Deg2Rad(MinYaw), Deg2Rad(MaxYaw), TFloat.Inst),
                new Range<float>(Deg2Rad(MinPitch), Deg2Rad(MaxPitch), TFloat.Inst),
                loggerFactory);

        protected abstract Validation<string, T> CreateService(
            Range<float> yawRange,
            Range<float> pitchRange,
            ILoggerFactory loggerFactory);
    }
}
