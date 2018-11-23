using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Motion;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Sensor
{
    public class PairedEyeSightFactory : TurretLikeFactory<PairedEyeSight>
    {
        [Service]
        public Option<Skeleton> Skeleton { get; set; }

        [Service]
        public Option<IAnimationManager> AnimationManager { get; set; }

        [Export]
        public string HeadBone { get; set; } = "head";

        [Export]
        public string EyeBoneLeft { get; set; } = "eye_L";

        [Export]
        public string EyeBoneRight { get; set; } = "eye_R";

        public PairedEyeSightFactory()
        {
            MinYaw = -80;
            MaxYaw = 80;
            MinPitch = -70;
            MaxPitch = 80;
        }

        protected override Validation<string, PairedEyeSight> CreateService(
            Range<float> yawRange, Range<float> pitchRange, ILogger logger)
        {
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return from skeleton in Skeleton
                    .ToValidation("Failed to find the skeleton.")
                from animationManager in AnimationManager
                    .ToValidation("Failed to find the animation manager.")
                from headBone in HeadBone.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the head bone.")
                from neckBone in Optional(skeleton.GetBoneParent(headBone)).Filter(i => i > -1)
                    .ToValidation("Failed to find the neck bone.")
                from leftEyeBone in EyeBoneLeft.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the left eye bone.")
                from rightEyeBone in EyeBoneRight.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the right eye bone.")
                select new PairedEyeSight(
                    skeleton,
                    animationManager,
                    headBone,
                    neckBone,
                    rightEyeBone,
                    leftEyeBone,
                    yawRange,
                    pitchRange,
                    Active,
                    logger);
        }
    }
}
