using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion
    {
        public const string WalkBlendNode = "Walk";

        public const string ForwardBlendNode = "Forward-Backward";

        public const string SideBlendNode = "Left-Right";

        [Service]
        public IAnimationStateManager AnimationManager { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        [Export, NotNull]
        public string PositionBone { get; set; } = "Position";

        private int _boneIndex;

        private Transform _initialTransform;

        private Transform _offset = new Transform(Basis.Identity, Vector3.Zero);

        private Transform _lastPose = new Transform(Basis.Identity, Vector3.Zero);

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            _boneIndex = Skeleton.FindBone(PositionBone);

            Debug.Assert(_boneIndex != -1, $"Failed to find a bone named '{PositionBone}'.");

            _initialTransform = Skeleton.GetBoneTransform(_boneIndex);

            AnimationManager.OnBeforeAdvance
                .Subscribe(_ => OnBeforeAnimation())
                .AddTo(this);

            AnimationManager.OnAdvance
                .Subscribe(OnAnimation)
                .AddTo(this);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Subscribe(_ => ResetAnimations())
                .AddTo(this);

            AnimationManager.Player
                .GetAnimationList()
                .Select(AnimationManager.Player.GetAnimation)
                .Where(a => a.Loop)
                .ToList()
                .ForEach(AddTrack);

            Reset();
        }

        private void AddTrack(Godot.Animation animation)
        {
            var args = new Dictionary<object, object>
            {
                {"method", "Reset"},
                {"args", new string[0]}
            };

            var track = animation.AddTrack(Godot.Animation.TrackType.Method);

            animation.TrackSetPath(track, GetPath());
            animation.TrackInsertKey(track, animation.Length, args);
        }

        protected override Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var player = AnimationManager.TreePlayer;

            var momentum = Math.Abs(velocity.x) + Math.Abs(velocity.z);

            if (momentum > 0)
            {
                var ratio = Math.Abs(velocity.z) / momentum;

                player.Blend2NodeSetAmount(WalkBlendNode, ratio);
            }

            player.Blend3NodeSetAmount(ForwardBlendNode, -velocity.z);
            player.Blend3NodeSetAmount(SideBlendNode, velocity.x);

            var rotation = new Transform(_offset.basis, new Vector3());

            Debug.Assert(Target != null, "Target != null");

            Target.GlobalTransform = rotation * Target.GlobalTransform;
            Target.RotateObjectLocal(Vector3.Up, rotationalVelocity.y);

            return _offset.origin / delta;
        }

        protected virtual void ResetAnimations()
        {
            var player = AnimationManager.TreePlayer;

            player.Blend2NodeSetAmount(WalkBlendNode, 0);
            player.Blend3NodeSetAmount(ForwardBlendNode, 0);
            player.Blend3NodeSetAmount(SideBlendNode, 0);
        }

        protected virtual void OnBeforeAnimation()
        {
            _lastPose = Skeleton.GetBoneTransform(_boneIndex);
        }

        protected virtual void OnAnimation(float delta)
        {
            var pose = Skeleton.GetBoneTransform(_boneIndex);
            var poseDelta = _lastPose.Inverse() * pose;

            var basis = poseDelta.basis;
            var origin = Skeleton.GlobalTransform.Xform(poseDelta.origin) - Skeleton.GlobalTransform.origin;

            _offset = new Transform(basis, origin);
            _lastPose = pose;

            Skeleton.SetBonePose(_boneIndex, new Transform(Basis.Identity, new Vector3()));
        }

        [UsedImplicitly]
        public void Reset()
        {
            _lastPose = _initialTransform;
        }
    }
}
