using AlleyCat.Autowire;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class RootMotionPlayer : AnimationPlayer
    {
        [Node]
        public Skeleton Skeleton { get; private set; }

        [Export, NotNull]
        public string RootBone { get; set; } = "Position";

        public Transform Offset { get; private set; }

        [Export, UsedImplicitly] private NodePath _skeleton = "..";

        private Transform _initialTransform;

        private Transform _lastPose;

        private int _positionIndex;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _positionIndex = Skeleton.FindBone(RootBone);
            _initialTransform = Skeleton.GetBoneTransform(_positionIndex);

            Reset();
        }

        public void ApplyOffset()
        {
            var pose = Skeleton.GetBoneTransform(_positionIndex);

            var delta = _lastPose.Inverse() * pose;

            var basis = delta.basis;
            var origin = Skeleton.GlobalTransform.Xform(delta.origin) - Skeleton.GlobalTransform.origin;

            Offset = new Transform(basis, origin);

            _lastPose = pose;

            Skeleton.SetBonePose(_positionIndex, new Transform(Basis.Identity, new Vector3()));
        }

        public void RecordOffset()
        {
            _lastPose = Skeleton.GetBoneTransform(_positionIndex);
        }

        [UsedImplicitly]
        public void Reset()
        {
            _lastPose = _initialTransform;
        }
    }
}
