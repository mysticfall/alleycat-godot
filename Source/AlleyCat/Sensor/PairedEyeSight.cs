using System;
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
    public class PairedEyeSight : GameNode, IPairedEyeSight
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public Skeleton Skeleton { get; }

        public IAnimationStateManager AnimationManager { get; }

        public Transform Head => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(HeadBone);

        public Transform Neck => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(NeckBone);

        public Transform Chest => Skeleton.GlobalTransform * Skeleton.GetBoneGlobalPose(ChestBone);

        public Transform LeftEye => LeftEyeMarker.GlobalTransform;

        public Transform RightEye => RightEyeMarker.GlobalTransform;

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

        public virtual Range<float> YawRange => NeckYawRange + HeadYawRange + EyesYawRange;

        public virtual Range<float> PitchRange => NeckPitchRange + HeadPitchRange + EyesPitchRange;

        public Vector3 Origin => Neck.origin;

        public Vector3 Up => (Neck.origin - Chest.origin).Normalized();

        public Vector3 Forward => (Chest.basis * ChestOrientation).Forward();

        public Vector3 Right => Forward.Cross(Up);

        protected SeekableAnimator HorizontalEyesControl { get; }

        protected SeekableAnimator VerticalEyesControl { get; }

        protected int HeadBone { get; }

        protected int NeckBone { get; }

        protected int ChestBone { get; }

        protected Marker RightEyeMarker { get; }

        protected Marker LeftEyeMarker { get; }

        protected Basis HeadOrientation { get; }

        protected Basis NeckOrientation { get; }

        protected Basis ChestOrientation { get; }

        protected ITimeSource TimeSource { get; }

        private readonly BehaviorSubject<bool> _active;

        private Seq<Chain> _chains;

        public PairedEyeSight(
            Skeleton skeleton,
            IAnimationStateManager animationManager,
            SeekableAnimator horizontalEyesControl,
            SeekableAnimator verticalEyesControl,
            Marker rightEyeMarker,
            Marker leftEyeMarker,
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
            Ensure.That(horizontalEyesControl, nameof(horizontalEyesControl)).IsNotNull();
            Ensure.That(verticalEyesControl, nameof(verticalEyesControl)).IsNotNull();
            Ensure.That(rightEyeMarker, nameof(rightEyeMarker)).IsNotNull();
            Ensure.That(leftEyeMarker, nameof(leftEyeMarker)).IsNotNull();
            Ensure.That(headBone, nameof(headBone)).IsGte(0);
            Ensure.That(neckBone, nameof(neckBone)).IsGte(0);
            Ensure.That(chestBone, nameof(chestBone)).IsGte(0);
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Skeleton = skeleton;
            AnimationManager = animationManager;

            HorizontalEyesControl = horizontalEyesControl;
            VerticalEyesControl = verticalEyesControl;

            RightEyeMarker = rightEyeMarker;
            LeftEyeMarker = leftEyeMarker;

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

            var neck = new BoneChain(
                () => new Transform(Chest.basis * ChestOrientation, Neck.origin),
                NeckYawRange,
                NeckPitchRange,
                Skeleton,
                NeckBone);

            var head = new BoneChain(
                () => new Transform(Neck.basis * NeckOrientation, Head.origin),
                HeadYawRange,
                HeadPitchRange,
                Skeleton,
                HeadBone);

            var eyes = new AnimatedChain(
                () => new Transform(Head.basis * HeadOrientation, Viewpoint),
                EyesYawRange,
                EyesPitchRange,
                HorizontalEyesControl,
                VerticalEyesControl);

            _chains = Seq<Chain>(neck, head, eyes);

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
            var transform = new Transform(Chest.basis * ChestOrientation, Neck.origin);

            var v = LookTarget
                .Map(t => CalculateRotation(t, transform, Vector2.Zero))
                .IfNone(Vector2.Zero);

            var yawDistance = Min(YawRange.Distance(v.x), 1f);
            var pitchDistance = Min(PitchRange.Distance(v.y), 1f);

            var offset = new Vector2(0, Deg2Rad(-15f));
            var influence = new Vector2(yawDistance, pitchDistance);

            _chains.Iter(c => c.Rotate(LookTarget, offset, influence));
        }

        public abstract class Chain
        {
            public Func<Transform> Transform { get; }

            public Range<float> YawRange { get; }

            public Range<float> PitchRange { get; }

            public Chain(
                Func<Transform> transform,
                Range<float> yawRange,
                Range<float> pitchRange)
            {
                Transform = transform;
                YawRange = yawRange;
                PitchRange = pitchRange;
            }

            public abstract void Rotate(Option<Vector3> target, Vector2 offset, Vector2 influence);
        }

        public class BoneChain : Chain
        {
            public Skeleton Skeleton { get; }

            public int Bone { get; }

            public BoneChain(
                Func<Transform> transform,
                Range<float> yawRange,
                Range<float> pitchRange,
                Skeleton skeleton,
                int bone) : base(transform, yawRange, pitchRange)
            {
                Skeleton = skeleton;
                Bone = bone;
            }

            public override void Rotate(Option<Vector3> target, Vector2 offset, Vector2 influence)
            {
                var transform = Transform();

                //FIXME: A temporary workaround until godotengine/godot#33552 gets fixed.
                Skeleton.SetBoneGlobalPoseOverride(Bone, Godot.Transform.Identity, 0f, false);

                target.Iter(t =>
                {
                    var v = CalculateRotation(t, transform, offset) * influence;

                    var h = Basis.Identity.Rotated(Vector3.Up, YawRange.Clamp(v.x));
                    var r = h.Xform(Vector3.Right);

                    var global = Skeleton.GetBoneGlobalPose(Bone);

                    var rotation = h.Rotated(r, PitchRange.Clamp(v.y));
                    var rotated = new Transform(rotation * global.basis, global.origin);

                    Skeleton.SetBoneGlobalPoseOverride(Bone, rotated, 1f, true);
                });
            }
        }

        public class AnimatedChain : Chain
        {
            public SeekableAnimator HorizontalControl { get; }

            public SeekableAnimator VerticalControl { get; }

            public AnimatedChain(
                Func<Transform> transform,
                Range<float> yawRange,
                Range<float> pitchRange,
                SeekableAnimator horizontalControl,
                SeekableAnimator verticalControl) : base(transform, yawRange, pitchRange)
            {
                HorizontalControl = horizontalControl;
                VerticalControl = verticalControl;
            }

            public override void Rotate(Option<Vector3> target, Vector2 offset, Vector2 influence)
            {
                Vector2 GetMorph(Vector3 t)
                {
                    var rotation = CalculateRotation(t, Transform(), offset) * influence;

                    var horizontal = (1f - YawRange.Ratio(YawRange.Clamp(rotation.x))) * 0.5f;
                    var vertical = (1f + PitchRange.Ratio(PitchRange.Clamp(rotation.y))) * 0.5f;

                    return new Vector2(horizontal, vertical);
                }

                var morph = target.Map(GetMorph).IfNone(new Vector2(0.5f, 0.5f));

                HorizontalControl.Position = morph.x;
                VerticalControl.Position = morph.y;
            }
        }

        private static Vector2 CalculateRotation(Vector3 target, Transform transform, Vector2 offset)
        {
            var basis = transform.basis;
            var origin = transform.origin;

            var up = basis.Xform(Vector3.Up);
            var right = basis.Xform(Vector3.Right);
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
