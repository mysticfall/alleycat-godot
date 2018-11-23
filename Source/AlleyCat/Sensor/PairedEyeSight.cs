using System;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Motion;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Sensor
{
    public class PairedEyeSight : TurretLike, IPairedEyeSight
    {
        public Skeleton Skeleton { get; }

        public IAnimationManager AnimationManager { get; }

        public Transform Head => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(HeadBone);

        public Transform Neck => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(NeckBone);

        public Transform LeftEye => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(LeftEyeBone);

        public Transform RightEye => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(RightEyeBone);

        public Vector3 Viewpoint => (LeftEye.origin + RightEye.origin) / 2f;

        public Vector3 LookDirection => (Head.basis * HeadOrientation).Xform(Vector3.Forward);

        public override Vector3 Origin => Head.origin;

        public override Vector3 Up => InitialRotation.Up();

        public override Vector3 Forward => InitialRotation.Forward();

        protected Basis InitialRotation
        {
            get
            {
                var neckPose = Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(NeckBone);
                return (neckPose * RestPose).basis * HeadOrientation;
            }
        }

        protected int HeadBone { get; }

        protected int NeckBone { get; }

        protected int LeftEyeBone { get; }

        protected int RightEyeBone { get; }

        protected Transform RestPose { get; }

        protected Basis HeadOrientation { get; }

        protected Basis NeckOrientation { get; }

        public PairedEyeSight(
            Skeleton skeleton,
            IAnimationManager animationManager,
            int headBone,
            int neckBone,
            int rightEyeBone,
            int leftEyeBone,
            Range<float> yawRange,
            Range<float> pitchRange,
            bool active,
            ILogger logger) : base(yawRange, pitchRange, active, logger)
        {
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();
            Ensure.That(animationManager, nameof(animationManager)).IsNotNull();
            Ensure.That(headBone, nameof(headBone)).IsGte(0);
            Ensure.That(neckBone, nameof(neckBone)).IsGte(0);
            Ensure.That(rightEyeBone, nameof(rightEyeBone)).IsGte(0);
            Ensure.That(leftEyeBone, nameof(leftEyeBone)).IsGte(0);

            Skeleton = skeleton;
            AnimationManager = animationManager;
            HeadBone = headBone;
            NeckBone = neckBone;
            LeftEyeBone = leftEyeBone;
            RightEyeBone = rightEyeBone;

            RestPose = Skeleton.GetBoneRest(HeadBone);

            HeadOrientation = DetectOrientation(HeadBone);
            NeckOrientation = DetectOrientation(NeckBone);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            AnimationManager
                .OnAdvance
                .Subscribe(OnAnimation)
                .DisposeWith(this);
        }

        private Basis DetectOrientation(int bone)
        {
            var rotation = Skeleton.GetBoneGlobalPose(bone).basis;

            var up = rotation.XformInv(Vector3.Up).ClosestGlobalAxis();
            var forward = rotation.XformInv(Vector3.Forward).ClosestGlobalAxis();

            return BasisExtensions.CreateFromAxes(forward.Cross(up), up, forward * -1).Inverse();
        }

        protected virtual void OnAnimation(float delta)
        {
            var rotation = Basis.Identity.Rotated(HeadOrientation.Up(), Yaw);
            var right = rotation.Xform(HeadOrientation.Right());

            rotation = rotation.Rotated(right, Pitch);

            Skeleton.SetBonePose(HeadBone, new Transform(rotation, Vector3.Zero));
        }

        public void LookAt(Vector3 target)
        {
            var initial = InitialRotation;

            var transform = new Transform(initial, Vector3.Zero).LookingAt(target, initial.Up());
            var euler = (initial.Inverse() * transform.basis).GetEuler();

            Yaw = euler.y;
            Pitch = euler.x;
        }
    }
}
