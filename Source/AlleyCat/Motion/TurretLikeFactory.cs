using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public abstract class TurretLikeFactory<T> : GameObjectFactory<T> where T : TurretLike
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

        protected override Validation<string, T> CreateService(ILogger logger) =>
            CreateService(
                new Range<float>(Mathf.Deg2Rad(MinYaw), Mathf.Deg2Rad(MaxYaw)),
                new Range<float>(Mathf.Deg2Rad(MinPitch), Mathf.Deg2Rad(MaxPitch)),
                logger);

        protected abstract Validation<string, T> CreateService(
            Range<float> yawRange, 
            Range<float> pitchRange, 
            ILogger logger);
    }
}
