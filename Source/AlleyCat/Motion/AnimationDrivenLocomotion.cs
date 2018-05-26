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
        public const string MoveTransition = "Idle-Move";

        public const string DirectionTransition = "Move";

        public const string ScaleSpeed = "Speed";

        public const int TransitionIdle = 0;

        public const int TransitionMove = 1;

        public const int TransitionForward = 0;

        public const int TransitionBackward = 1;

        public const int TransitionStrafe = 2;

        public const string ForwardBlend = "Forward";

        public const string BackwardBlend = "Backward";

        public const string StrafeTransition = "Strafe";

        public const int TransitionLeft = 0;

        public const int TransitionRight = 1;

        [Service]
        public IAnimationStateManager AnimationManager { get; private set; }

        public AnimationTreePlayer TreePlayer => AnimationManager.TreePlayer;

        [Service]
        public Skeleton Skeleton { get; private set; }

        [Export, NotNull]
        public string PositionBone { get; set; } = "root";

        private int _boneIndex;

        private Transform _initialTransform;

        private Transform _offset = new Transform(Basis.Identity, Vector3.Zero);

        private Transform _lastPose = new Transform(Basis.Identity, Vector3.Zero);
        private Label _label;

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();
            _label = new Label();
            AddChild(_label);
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
            animation.TrackInsertKey(track, 0, args);
            animation.TrackInsertKey(track, animation.Length, args);
        }

        protected override void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            base.Process(delta, velocity, rotationalVelocity);

            Debug.Assert(Target != null, "Target != null");

            var transform = Target.GlobalTransform;
            var basis = transform.basis * _offset.basis *
                        Basis.Identity.Rotated(Vector3.Up, rotationalVelocity.y * delta);

            Target.GlobalTransform = new Transform(basis, transform.origin);
        }

        protected override Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var speed = velocity.Length();
            var direction = velocity.Normalized();

            if (speed > 0)
            {
//                if (Mathf.Abs(direction.z) > 0)
//                {
                var forward = direction.z <= 0;

                TreePlayer.TransitionNodeSetCurrent(DirectionTransition,
                    forward ? TransitionForward : TransitionBackward);
                TreePlayer.Blend3NodeSetAmount(forward ? ForwardBlend : BackwardBlend, direction.x);
//                }
//                else
//                {
//                    TreePlayer.TransitionNodeSetCurrent(DirectionTransition, TransitionStrafe);
//                    TreePlayer.TransitionNodeSetCurrent(StrafeTransition,
//                        direction.x < 0 ? TransitionLeft : TransitionRight);
//                }

                TreePlayer.TransitionNodeSetCurrent(MoveTransition, TransitionMove);
                TreePlayer.TimescaleNodeSetScale(ScaleSpeed, speed);
            }
            else
            {
                TreePlayer.TransitionNodeSetCurrent(MoveTransition, TransitionIdle);
            }

            return _offset.origin / delta;
        }

        protected virtual void ResetAnimations()
        {
            TreePlayer.TransitionNodeSetCurrent(MoveTransition, TransitionIdle);
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
