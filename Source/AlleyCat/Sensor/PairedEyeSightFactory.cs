using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static Godot.Mathf;

namespace AlleyCat.Sensor
{
    public class PairedEyeSightFactory : GameObjectFactory<PairedEyeSight>
    {
        [Export]
        public bool Active { get; set; } = true;

        [Service]
        public Option<Skeleton> Skeleton { get; set; }

        [Service]
        public Option<IAnimationManager> AnimationManager { get; set; }

        [Export]
        public string EyeBoneLeft { get; set; } = "eye_L";

        [Export]
        public string EyeBoneRight { get; set; } = "eye_R";

        [Export]
        public string HeadBone { get; set; } = "head";

        [Export]
        public string NeckBone { get; set; } = "neck";

        [Export]
        public string ChestBone { get; set; } = "spine03";

        [Export(PropertyHint.Range, "-90,0")]
        public float MinEyesYaw { get; set; } = -40;

        [Export(PropertyHint.Range, "0,90")]
        public float MaxEyesYaw { get; set; } = 40;

        [Export(PropertyHint.Range, "-90,0")]
        public float MinEyesPitch { get; set; } = -20;

        [Export(PropertyHint.Range, "0,90")]
        public float MaxEyesPitch { get; set; } = 30;

        [Export(PropertyHint.Range, "-90,0")]
        public float MinHeadYaw { get; set; } = -35;

        [Export(PropertyHint.Range, "0,90")]
        public float MaxHeadYaw { get; set; } = 35;

        [Export(PropertyHint.Range, "-90,0")]
        public float MinHeadPitch { get; set; } = -20;

        [Export(PropertyHint.Range, "0,90")]
        public float MaxHeadPitch { get; set; } = 30;

        [Export(PropertyHint.Range, "-90,0")]
        public float MinNeckYaw { get; set; } = -30;

        [Export(PropertyHint.Range, "0,90")]
        public float MaxNeckYaw { get; set; } = 30;

        [Export(PropertyHint.Range, "-90,0")]
        public float MinNeckPitch { get; set; } = -20;

        [Export(PropertyHint.Range, "0,90")]
        public float MaxNeckPitch { get; set; } = 20;

        protected override Validation<string, PairedEyeSight> CreateService(ILoggerFactory loggerFactory)
        {
            return from skeleton in Skeleton
                    .ToValidation("Failed to find the skeleton.")
                from animationManager in AnimationManager
                    .ToValidation("Failed to find the animation manager.")
                from leftEyeBone in EyeBoneLeft.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the left eye bone.")
                from rightEyeBone in EyeBoneRight.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the right eye bone.")
                from headBone in HeadBone.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the head bone.")
                from neckBone in NeckBone.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the neck bone.")
                from chestBone in ChestBone.TrimToOption()
                    .Map(skeleton.FindBone).Filter(i => i > -1)
                    .ToValidation("Failed to find the chest bone.")
                select new PairedEyeSight(
                    skeleton,
                    animationManager,
                    rightEyeBone,
                    leftEyeBone,
                    headBone,
                    neckBone,
                    chestBone,
                    new Range<float>(Deg2Rad(MinEyesYaw), Deg2Rad(MaxEyesYaw)),
                    new Range<float>(Deg2Rad(MinEyesPitch), Deg2Rad(MaxEyesPitch)),
                    new Range<float>(Deg2Rad(MinHeadYaw), Deg2Rad(MaxHeadYaw)),
                    new Range<float>(Deg2Rad(MinHeadPitch), Deg2Rad(MaxHeadPitch)),
                    new Range<float>(Deg2Rad(MinNeckYaw), Deg2Rad(MaxNeckYaw)),
                    new Range<float>(Deg2Rad(MinNeckPitch), Deg2Rad(MaxNeckPitch)),
                    this,
                    Active,
                    loggerFactory);
        }
    }
}
