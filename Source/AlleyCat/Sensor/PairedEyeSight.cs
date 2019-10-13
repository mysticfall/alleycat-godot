using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using static Godot.Mathf;

namespace AlleyCat.Sensor
{
    public class PairedEyeSight : GameObject, IPairedEyeSight
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public Skeleton Skeleton { get; }

        public IAnimationManager AnimationManager { get; }

        public Transform Head => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(HeadBone);

        public Transform Neck => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(NeckBone);

        public Transform Chest => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(ChestBone);

        public Transform LeftEye => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(LeftEyeBone);

        public Transform RightEye => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(RightEyeBone);

        public Vector3 Viewpoint => (LeftEye.origin + RightEye.origin) / 2f;

        public Option<Vector3> LookTarget { get; set; }

        public Vector3 LineOfSight => LookTarget.Map(t => (t - Viewpoint).Normalized())
            .IfNone((Head.basis * HeadOrientation).Xform(Vector3.Forward));

        public virtual Range<float> EyesYawRange { get; }

        public virtual Range<float> EyesPitchRange { get; }

        public virtual Range<float> HeadYawRange { get; }

        public virtual Range<float> HeadPitchRange { get; }

        public virtual Range<float> NeckYawRange { get; }

        public virtual Range<float> NeckPitchRange { get; }

        public Vector3 Origin => Neck.origin;

        public Vector3 Up => (Neck.origin - Chest.origin).Normalized();

        public Vector3 Forward => (Chest.basis * ChestOrientation).Forward();

        public Vector3 Right => Forward.Cross(Up);

        protected int HeadBone { get; }

        protected int NeckBone { get; }

        protected int ChestBone { get; }

        protected int LeftEyeBone { get; }

        protected int RightEyeBone { get; }

        protected Basis HeadOrientation { get; }

        protected Basis NeckOrientation { get; }

        protected Basis ChestOrientation { get; }

        protected ITimeSource TimeSource { get; }

        private readonly BehaviorSubject<bool> _active;

        public PairedEyeSight(
            Skeleton skeleton,
            IAnimationManager animationManager,
            int rightEyeBone,
            int leftEyeBone,
            int headBone,
            int neckBone,
            int chestBone,
            Range<float> eyesYawRange,
            Range<float> eyesPitchRange,
            Range<float> headYawRange,
            Range<float> headPitchRange,
            Range<float> neckYawRange,
            Range<float> neckPitchRange,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();
            Ensure.That(animationManager, nameof(animationManager)).IsNotNull();
            Ensure.That(rightEyeBone, nameof(rightEyeBone)).IsGte(0);
            Ensure.That(leftEyeBone, nameof(leftEyeBone)).IsGte(0);
            Ensure.That(headBone, nameof(headBone)).IsGte(0);
            Ensure.That(neckBone, nameof(neckBone)).IsGte(0);
            Ensure.That(chestBone, nameof(chestBone)).IsGte(0);
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Skeleton = skeleton;
            AnimationManager = animationManager;

            LeftEyeBone = leftEyeBone;
            RightEyeBone = rightEyeBone;
            HeadBone = headBone;
            NeckBone = neckBone;
            ChestBone = chestBone;

            EyesYawRange = eyesYawRange;
            EyesPitchRange = eyesPitchRange;
            HeadYawRange = headYawRange;
            HeadPitchRange = headPitchRange;
            NeckYawRange = neckYawRange;
            NeckPitchRange = neckPitchRange;

            TimeSource = timeSource;

            HeadOrientation = DetectOrientation(HeadBone);
            NeckOrientation = DetectOrientation(NeckBone);
            ChestOrientation = DetectOrientation(ChestBone);

            _active = CreateSubject(active);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            AnimationManager.OnAdvance
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(OnAnimation, this);
        }

        private Basis DetectOrientation(int bone)
        {
            var rotation = Skeleton.GetBoneGlobalPose(bone).basis;

            var up = rotation.XformInv(Vector3.Up).ClosestGlobalAxis();
            var forward = rotation.XformInv(Vector3.Forward).ClosestGlobalAxis();

            return new Basis(forward.Cross(up), up, forward * -1).Inverse();
        }

        protected virtual void OnAnimation(float delta)
        {
            LookTarget.Iter(target =>
            {
                var v = CalculateRotation(
                    target,
                    Neck.origin,
                    Chest.basis * ChestOrientation,
                    Vector2.Zero);

                var yawRange = new[] {NeckYawRange, HeadYawRange, EyesYawRange}.Aggregate((r1, r2) => r1 + r2);
                var pitchRange = new[] {NeckPitchRange, HeadPitchRange, EyesPitchRange}.Aggregate((r1, r2) => r1 + r2);

                var yawDistance = Min(yawRange.Distance(v.x), 1f);
                var pitchDistance = Min(pitchRange.Distance(v.y), 1f);

                ApplyRotation(
                    target,
                    Neck.origin,
                    Chest.basis * ChestOrientation,
                    new[] {NeckBone},
                    NeckYawRange,
                    NeckPitchRange,
                    new Vector2(0, Deg2Rad(-15f)),
                    new Vector2(yawDistance, pitchDistance));

                ApplyRotation(
                    target,
                    Head.origin,
                    Neck.basis * NeckOrientation,
                    new[] {HeadBone},
                    HeadYawRange,
                    HeadPitchRange,
                    new Vector2(0, Deg2Rad(-15f)),
                    new Vector2(yawDistance, pitchDistance));

                ApplyRotation(
                    target,
                    Viewpoint,
                    Head.basis * HeadOrientation,
                    new[] {LeftEyeBone, RightEyeBone},
                    EyesYawRange,
                    EyesPitchRange,
                    Vector2.Zero,
                    Vector2.One);
            });
        }

        protected virtual void ApplyRotation(
            Vector3 target,
            Vector3 origin,
            Basis reference,
            IEnumerable<int> bones,
            Range<float> yawRange,
            Range<float> pitchRange,
            Vector2 offset,
            Vector2 influence)
        {
            var v = CalculateRotation(target, origin, reference, offset) * influence;

            var h = Basis.Identity.Rotated(Vector3.Up, yawRange.Clamp(v.x));
            var r = h.Xform(Vector3.Right);

            var rotation = h.Rotated(r, pitchRange.Clamp(v.y));

            bones.Iter(i =>
            {
                var global = Skeleton.GetBoneGlobalPose(i);

                Skeleton.SetBoneGlobalPoseOverride(i, new Transform(rotation * global.basis, global.origin), 1f);
            });
        }

        protected virtual Vector2 CalculateRotation(
            Vector3 target,
            Vector3 origin,
            Basis reference,
            Vector2 offset)
        {
            var up = reference.Xform(Vector3.Up);
            var right = reference.Xform(Vector3.Right);
            var forward = up.Cross(right);

            var direction = (target - origin).Normalized();
            var cross = forward.Cross(direction);

            var plane = new Plane(origin, origin + forward, origin + right);
            var horizontal = (plane.Project(target) - origin).Normalized();

            var yaw = forward.AngleTo(horizontal) * Math.Sign(cross.Dot(up)) + offset.x;
            var pitch = direction.AngleTo(horizontal) * Math.Sign(cross.Dot(right)) + offset.y;

            return new Vector2(yaw, pitch);
        }
    }
}
