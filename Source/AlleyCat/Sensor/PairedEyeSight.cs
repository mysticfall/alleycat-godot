using System;
using System.Diagnostics;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Sensor
{
    [Singleton(typeof(IVision), typeof(IEyeSight), typeof(IPairedEyeSight))]
    public class PairedEyeSight : TurretLike, IPairedEyeSight
    {
        [Service]
        public Skeleton Skeleton { get; private set; }

        [Service]
        public IAnimationManager AnimationManager { get; private set; }

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

        protected int HeadBone { get; private set; }

        protected int NeckBone { get; private set; }

        protected int LeftEyeBone { get; private set; }

        protected int RightEyeBone { get; private set; }

        protected Transform RestPose { get; private set; }

        protected Basis HeadOrientation { get; private set; }

        protected Basis NeckOrientation { get; private set; }

        [Export, NotNull] private string _headBone = "head";

        [Export, NotNull] private string _eyeBoneLeft = "eye_L";

        [Export, NotNull] private string _eyeBoneRight = "eye_R";

        protected override void OnInitialize()
        {
            base.OnInitialize();

            HeadBone = Skeleton.FindBone(_headBone);

            LeftEyeBone = Skeleton.FindBone(_eyeBoneLeft);
            RightEyeBone = Skeleton.FindBone(_eyeBoneRight);

            Debug.Assert(HeadBone != -1, "Failed to find the head bone");
            Debug.Assert(LeftEyeBone != -1 || RightEyeBone != -1, "Failed to find the eye bones");

            NeckBone = Skeleton.GetBoneParent(HeadBone);

            Debug.Assert(HeadBone != -1, "Failed to find the neck bone");

            RestPose = Skeleton.GetBoneRest(HeadBone);

            AnimationManager.OnAdvance.Subscribe(OnAnimation).AddTo(this);

            HeadOrientation = DetectOrientation(HeadBone);
            NeckOrientation = DetectOrientation(NeckBone);
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
