using System;
using System.Diagnostics;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion, IAnimationPostProcessor
    {
        public const string WalkBlendNode = "Walk";

        public const string ForwardBlendNode = "Forward-Backward";

        public const string SideBlendNode = "Left-Right";

        [Service]
        public PostProcessingAnimationPlayer AnimationPlayer { get; private set; }

        [Service]
        public AnimationTreePlayer AnimationTreePlayer { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        [Export, NotNull]
        public string PositionBone { get; set; } = "Position";

        private int _boneIndex;

        private Transform _initialTransform;

        private Transform _offset;

        private Transform _lastPose;

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            AnimationTreePlayer.Active = false;
            AnimationPlayer.PlaybackActive = false;
            AnimationPlayer.Processors.Add(this);

            _boneIndex = Skeleton.FindBone(PositionBone);

            Debug.Assert(_boneIndex != -1, $"Failed to find a bone named '{PositionBone}'.");

            _initialTransform = Skeleton.GetBoneTransform(_boneIndex);

            Reset();
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            AnimationPlayer.Processors.Remove(this);
        }

        protected override Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var momentum = Math.Abs(velocity.x) + Math.Abs(velocity.z);

            if (momentum > 0)
            {
                var ratio = Math.Abs(velocity.z) / momentum;

                AnimationTreePlayer.Blend2NodeSetAmount(WalkBlendNode, ratio);
            }

            AnimationTreePlayer.Blend3NodeSetAmount(ForwardBlendNode, -velocity.z);
            AnimationTreePlayer.Blend3NodeSetAmount(SideBlendNode, velocity.x);

            AnimationTreePlayer.Advance(0);

            AnimationPlayer.BeforeFrame();

            AnimationTreePlayer.Advance(delta);

            AnimationPlayer.AfterFrame(delta);

            var rotation = new Transform(_offset.basis, new Vector3());

            Debug.Assert(Target != null, "Target != null");

            Target.GlobalTransform = rotation * Target.GlobalTransform;
            Target.RotateObjectLocal(Vector3.Up, rotationalVelocity.y);

            return _offset.origin / delta;
        }

        public void BeforeFrame(PostProcessingAnimationPlayer player)
        {
            Ensure.Any.IsNotNull(player, nameof(player));

            _lastPose = Skeleton.GetBoneTransform(_boneIndex);
        }

        public void AfterFrame(PostProcessingAnimationPlayer player, float delta)
        {
            Ensure.Any.IsNotNull(player, nameof(player));

            var pose = Skeleton.GetBoneTransform(_boneIndex);
            var poseDelta = _lastPose.Inverse() * pose;

            var basis = poseDelta.basis;
            var origin = Skeleton.GlobalTransform.Xform(poseDelta.origin) - Skeleton.GlobalTransform.origin;

            _offset = new Transform(basis, origin);
            _lastPose = pose;

            Skeleton.SetBonePose(_boneIndex, new Transform(Basis.Identity, new Vector3()));
        }

        public void Reset()
        {
            _lastPose = _initialTransform;
        }
    }
}
