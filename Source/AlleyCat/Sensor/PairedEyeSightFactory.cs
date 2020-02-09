using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Logging;
using static Godot.Mathf;

namespace AlleyCat.Sensor
{
    public class PairedEyeSightFactory : GameNodeFactory<PairedEyeSight>
    {
        [Export]
        public bool Active { get; set; } = true;

        [Service]
        public Option<Skeleton> Skeleton { get; set; }

        [Service]
        public Option<IAnimationStateManager> AnimationManager { get; set; }

        [Service]
        public IEnumerable<Marker> Markers { get; set; }

        [Export]
        public string HorizontalEyesControl { get; set; } = "Eyes Control/Horizontal";

        [Export]
        public string VerticalEyesControl { get; set; } = "Eyes Control/Vertical";

        [Export]
        public string RightEyeMarker { get; set; } = "Right Eye";

        [Export]
        public string LeftEyeMarker { get; set; } = "Left Eye";

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
                from horizontalEyesControl in HorizontalEyesControl.TrimToOption()
                    .Bind(animationManager.FindSeekableAnimator)
                    .ToValidation("Failed to find an animation control for horizontal eyes movement.")
                from verticalEyesControl in VerticalEyesControl.TrimToOption()
                    .Bind(animationManager.FindSeekableAnimator)
                    .ToValidation("Failed to find an animation control for vertical eyes movement.")
                from rightEye in Markers.Find(m => RightEyeMarker.TrimToOption().Contains(m.Key))
                    .ToValidation("Failed to find the right eye marker.")
                from leftEye in Markers.Find(m => LeftEyeMarker.TrimToOption().Contains(m.Key))
                    .ToValidation("Failed to find the left eye marker.")
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
                    horizontalEyesControl,
                    verticalEyesControl,
                    rightEye,
                    leftEye,
                    headBone,
                    neckBone,
                    chestBone,
                    new Range<float>(Deg2Rad(MinEyesYaw), Deg2Rad(MaxEyesYaw), TFloat.Inst),
                    new Range<float>(Deg2Rad(MinEyesPitch), Deg2Rad(MaxEyesPitch), TFloat.Inst),
                    new Range<float>(Deg2Rad(MinHeadYaw), Deg2Rad(MaxHeadYaw), TFloat.Inst),
                    new Range<float>(Deg2Rad(MinHeadPitch), Deg2Rad(MaxHeadPitch), TFloat.Inst),
                    new Range<float>(Deg2Rad(MinNeckYaw), Deg2Rad(MaxNeckYaw), TFloat.Inst),
                    new Range<float>(Deg2Rad(MinNeckPitch), Deg2Rad(MaxNeckPitch), TFloat.Inst),
                    this,
                    Active,
                    loggerFactory);
        }
    }
}
