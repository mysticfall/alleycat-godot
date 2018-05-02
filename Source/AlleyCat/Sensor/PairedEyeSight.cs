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
    public class PairedEyeSight : Rotatable, IPairedEyeSight
    {
        [Service]
        public Skeleton Skeleton { get; private set; }

        [Service]
        public IAnimationManager AnimationManager { get; private set; }

        public Transform Head => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(_headIndex);

        public Transform LeftEye => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(_eyeIndexLeft);

        public Transform RightEye => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(_eyeIndexRight);

        public override Vector3 Origin => (LeftEye.origin + RightEye.origin) / 2f;

        public override Vector3 Up => (Head.basis * _basis).Xform(Vector3.Up);

        public override Vector3 Forward => (Head.basis * _basis).Xform(Vector3.Forward);

        [Export, NotNull] private string _headBone = "Head";

        [Export, NotNull] private string _eyeBoneLeft = "Eye.L";

        [Export, NotNull] private string _eyeBoneRight = "Eye.R";

        private int _headIndex;

        private int _eyeIndexLeft;

        private int _eyeIndexRight;

        private Basis _basis;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _headIndex = Skeleton.FindBone(_headBone);

            _eyeIndexLeft = Skeleton.FindBone(_eyeBoneLeft);
            _eyeIndexRight = Skeleton.FindBone(_eyeBoneRight);

            Debug.Assert(_headIndex != -1, "Failed to find the head bone");
            Debug.Assert(_eyeIndexLeft != -1 || _eyeIndexRight != -1, "Failed to find the eye bones");

            AnimationManager.OnAdvance.Subscribe(OnAnimation).AddTo(this);

            var rotation = Skeleton.GetBoneGlobalPose(_headIndex).basis;

            var up = rotation.XformInv(Vector3.Up).ClosestGlobalAxis();
            var forward = rotation.XformInv(Vector3.Forward).ClosestGlobalAxis();

            _basis = new Basis(forward.Cross(up), up, forward * -1).Inverse();
        }

        protected virtual void OnAnimation(float delta)
        {
            var rotation = Basis.Identity.Rotated(_basis.Up(), Yaw);
            var right = rotation.Xform(_basis.Right());

            rotation = rotation.Rotated(right, Pitch);

            Skeleton.SetBonePose(_headIndex, new Transform(rotation, Vector3.Zero));
        }
        
        public void LookAt(Vector3 target)
        {
            throw new NotImplementedException();
        }
    }
}
