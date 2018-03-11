using System;
using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Sensor
{
    [Singleton(typeof(IVision), typeof(IEyeSight), typeof(IPairedEyeSight))]
    public class PairedEyeSight : AutowiredNode, IPairedEyeSight
    {
        [Export]
        public bool Active { get; set; } = true;

        public Transform Head => Skeleton.GlobalTransform *
                                 new Transform(Skeleton.GetBoneTransform(_headIndex).basis,
                                     Skeleton.GetBoneGlobalPose(_headIndex).origin);

        public Transform LeftEye => Skeleton.GlobalTransform *
                                    new Transform(Skeleton.GetBoneTransform(_eyeIndexLeft).basis,
                                        Skeleton.GetBoneGlobalPose(_eyeIndexLeft).origin);

        public Transform RightEye => Skeleton.GlobalTransform *
                                     new Transform(Skeleton.GetBoneTransform(_eyeIndexRight).basis,
                                         Skeleton.GetBoneGlobalPose(_eyeIndexRight).origin);

        [Service]
        public Skeleton Skeleton { get; private set; }

        public Vector3 Origin
        {
            get
            {
                var leftEye = Skeleton.GetBoneGlobalPose(_eyeIndexLeft).origin;
                var rightEye = Skeleton.GetBoneGlobalPose(_eyeIndexRight).origin;

                return Skeleton.GlobalTransform.Xform((leftEye + rightEye) / 2f);
            }
        }

        public Vector3 Forward
        {
            get
            {
                var bodyRotation = Skeleton.GlobalTransform.basis;
                var forward = Skeleton.GetBoneTransform(_headIndex).Forward();

                return bodyRotation.Xform(forward);
            }
        }

        public Vector3 Up
        {
            get
            {
                var bodyRotation = Skeleton.GlobalTransform.basis;
                var forward = Skeleton.GetBoneTransform(_headIndex).Up();

                return bodyRotation.Xform(forward);
            }
        }

        public Vector3 Right => Forward.Cross(Up);

        [Export, NotNull] private string _headBone = "Head";

        [Export, NotNull] private string _eyeBoneLeft = "LeftEye";

        [Export, NotNull] private string _eyeBoneRight = "RightEye";

        private int _headIndex;

        private int _eyeIndexLeft;

        private int _eyeIndexRight;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _headIndex = Skeleton.FindBone(_headBone);

            _eyeIndexLeft = Skeleton.FindBone(_eyeBoneLeft);
            _eyeIndexRight = Skeleton.FindBone(_eyeBoneRight);

            Debug.Assert(_headIndex != -1, "Failed to find the head bone");
            Debug.Assert(_eyeIndexLeft != -1 || _eyeIndexRight != -1, "Failed to find the eye bones");
        }

        public void LookAt(Vector3 target)
        {
            throw new NotImplementedException();
        }
    }
}
