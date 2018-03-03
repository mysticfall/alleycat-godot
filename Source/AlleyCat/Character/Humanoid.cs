using System;
using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions; 

namespace AlleyCat.Character
{
    public class Humanoid : Character
    {
        public override Vector3 Viewpoint
        {
            get
            {
                var leftEye = Skeleton.GetBoneGlobalPose(_eyeIndexLeft).origin;
                var rightEye = Skeleton.GetBoneGlobalPose(_eyeIndexRight).origin;

                return Skeleton.GlobalTransform.Xform((leftEye + rightEye) / 2f);
            }
        }

        public override Vector3 LookingAt
        {
            get
            {
                var bodyRotation = Skeleton.GlobalTransform.basis;
                var forward = Skeleton.GetBoneTransform(_headIndex).Forward();

                return bodyRotation.Xform(forward);
            }
        }

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
    }
}
