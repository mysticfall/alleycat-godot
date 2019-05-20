using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using AlleyCat.Motion;
using AlleyCat.Physics;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using Array = Godot.Collections.Array;

namespace AlleyCat.View
{
    public class HeadMountedView : TurretLike, IFirstPersonView, IAutoFocusingView
    {
        public Camera Camera { get; }

        public virtual Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        public override bool Valid => base.Valid && Character.IsSome && Camera.Current;

        public bool AutoActivate => true;

        public enum StabilizeMode
        {
            Never,
            WhileMoving,
            Always
        }

        public StabilizeMode Stabilization { get; set; } = StabilizeMode.WhileMoving;

        public Range<float> StabilizationFactor => new Range<float>(_minStabilization, _maxStabilization, TFloat.Inst);

        public float MaxStabilization
        {
            get => _maxStabilization;
            set => _maxStabilization = Mathf.Max(0, Mathf.Max(value, MinStabilization));
        }

        public float MinStabilization
        {
            get => _minStabilization;
            set => _minStabilization = Mathf.Max(0, Mathf.Min(value, MaxStabilization));
        }

        public float TransitionTime
        {
            get => _transitionTime;
            set => _transitionTime = Mathf.Max(value, 0);
        }

        public float VelocityThreshold
        {
            get => _velocityThreshold;
            set => _velocityThreshold = Mathf.Max(value, 0);
        }

        public Option<IEntity> FocusedObject { get; private set; }

        public IObservable<Option<IEntity>> OnFocusChange { get; }

        public float MaxFocalDistance
        {
            get => _maxFocalDistance;
            set => _maxFocalDistance = Mathf.Max(value, 0);
        }

        public float MaxDofDistance
        {
            get => _maxDofDistance;
            set => _maxDofDistance = Mathf.Max(value, 0);
        }

        public float FocusRange
        {
            get => _focusRange;
            set => _focusRange = Mathf.Max(value, 0);
        }

        public float FocusSpeed
        {
            get => _focusSpeed;
            set => _focusSpeed = Mathf.Max(value, 0);
        }

        public Option<Curve> NeckRotationCurve { get; set; }

        public Option<IVision> Vision => Character.Map<IVision>(c => c.Vision);

        public Vector3 Viewpoint => Vision.Map(v => v.Viewpoint).IfNone(Vector3.Zero);

        public Vector3 LineOfSight => Vision.Map(v => v.LineOfSight).IfNone(Vector3.Forward);

        public override Vector3 Origin => Vision.Map(v => v.Origin).IfNone(Vector3.Zero);

        public override Vector3 Up => Vision.Map(v => v.Up).IfNone(Vector3.Up);

        public override Vector3 Forward => Vision.Map(v => v.Forward).IfNone(Vector3.Forward);

        public override Range<float> PitchRange
        {
            get
            {
                var ratio = Math.Abs(Yaw / (Yaw > 0 ? YawRange.Max : YawRange.Min));
                var factor = NeckRotationCurve.Map(c => c.Interpolate(ratio)).IfNone(1f);
                var range = base.PitchRange;

                return new Range<float>(range.Min * factor, range.Max, TFloat.Inst);
            }
        }

        public float Offset { get; set; }

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        protected virtual IObservable<Vector2> RotationInput { get; }

        protected virtual IObservable<bool> DeactivateInput { get; }

        private readonly Option<IInputBindings> _rotationInput;

        private readonly Option<IInputBindings> _deactivateInput;

        private float _transitionTime = 2f;

        private float _velocityThreshold = 0.2f;

        private float _maxFocalDistance = 2f;

        private float _maxDofDistance = 5f;

        private float _focusRange = 3f;

        private float _focusSpeed = 100f;

        private float _minStabilization = 0.2f;

        private float _maxStabilization = 0.8f;

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        public HeadMountedView(
            Camera camera,
            Option<IHumanoid> character,
            Option<IInputBindings> rotationInput,
            Option<IInputBindings> deactivateInput,
            Range<float> yawRange,
            Range<float> pitchRange,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(yawRange, pitchRange, active, loggerFactory)
        {
            Ensure.That(camera, nameof(camera)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Camera = camera;

            ProcessMode = processMode;
            TimeSource = timeSource;

            RotationInput = rotationInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Select(v => v * 0.3f)
                .Where(_ => Valid);
            DeactivateInput = deactivateInput.Bind(i => i.FindTrigger().HeadOrNone())
                .MatchObservable(identity, Observable.Empty<bool>)
                .Where(_ => Valid);

            var onRayCast = TimeSource.OnPhysicsProcess
                .Where(_ => Active && Valid)
                .Select(_ => Viewpoint + LineOfSight * Mathf.Max(MaxFocalDistance, MaxDofDistance))
                .Select(to => Character
                    .Map(c => new Array {c.Spatial})
                    .Bind(filter => Camera.GetWorld().IntersectRay(Origin, to, filter)));

            onRayCast
                .Select(hit => hit.Select(h => Viewpoint.DistanceTo(h.GetPosition())).IfNone(float.MaxValue))
                .Buffer(
                    TimeSpan.FromMilliseconds(FocusSpeed),
                    TimeSpan.FromMilliseconds(10),
                    TimeSource.PhysicsScheduler)
                .Where(v => v.Any() && Active)
                .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(this.SetFocalDistance, this);

            OnFocusChange = onRayCast
                .Select(hit => hit.Where(h => Viewpoint.DistanceTo(h.GetPosition()) <= MaxFocalDistance))
                .Select(hit => hit.Bind(h => h.GetCollider().FindEntity()))
                .Select(entity => entity.Where(e => e.Valid && e.Visible))
                .DistinctUntilChanged();

            OnFocusChange
                .Do(v => this.LogDebug("Focusing on '{}'.", v))
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(current => FocusedObject = current, this);

            _character = CreateSubject(character);

            _rotationInput = rotationInput;
            _deactivateInput = deactivateInput;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnActiveStateChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(HandleActiveStateChange, this);

            RotationInput
                .Select(v => v * 0.05f)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Rotation -= v, this);

            DeactivateInput
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(_ => this.Deactivate(), this);

            Vector3 AcquireTarget(IHumanoid character) =>
                Origin + (character.GetGlobalTransform().basis * this.GetBasis()).Xform(Vector3.Forward);

            TimeSource.OnProcess(ProcessMode)
                .Where(_ => Active && Valid)
                .WithLatestFrom(OnCharacterChange.Select(c => c.ToObservable()).Switch(), (_, c) => c)
                .Select(AcquireTarget)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(r => Vision.Iter(v => v.LookTarget = r), this);

            InitializeStabilization();
        }

        private void HandleActiveStateChange(bool active)
        {
            _rotationInput.Iter(i => i.Active = active);
            _deactivateInput.Iter(i => i.Active = active);

            if (active)
            {
                this.EnableDof();
            }
            else if (Valid)
            {
                Vision.Iter(v => v.LookAt(None));
            }

            //TODO Should find a better way not to break the shadow and reflection.
            Character
                .Bind(c => c.Meshes)
                .Find(m => m.Name == "Head")
                .Iter(m => m.Visible = !active);
        }

        private void InitializeStabilization()
        {
            bool IsStablizationAllowed() =>
                Stabilization == StabilizeMode.Always || Stabilization != StabilizeMode.Never;

            var movingStateChange = OnCharacterChange
                .Select(c => c.ToObservable()).Switch()
                .Select(c => c.Locomotion.OnVelocityChange).Switch()
                .Select(v => v.Length() >= VelocityThreshold)
                .DistinctUntilChanged();

            var shouldStablize = movingStateChange
                .Select(v => v && IsStablizationAllowed());

            var transition = TimeSource.OnProcess(ProcessMode)
                .Zip(
                    shouldStablize.MostRecent(false),
                    (delta, stablizing) => stablizing ? delta : -delta)
                .Scan((time, delta) => Active ? Mathf.Max(0, Mathf.Min(TransitionTime, delta + time)) : 0)
                .Select(influence => influence / TransitionTime)
                .Select(StabilizationFactor.Clamp);

            Transform Stabilize(IHumanoid character, float ratio)
            {
                var vision = character.Vision;

                var up = vision.Up.LinearInterpolate(character.GetGlobalTransform().Up(), ratio);
                var origin = Viewpoint + vision.Forward * Offset;

                return new Transform(Basis.Identity, origin).LookingAt(origin + vision.LineOfSight * 10f, up);
            }

            TimeSource.OnProcess(ProcessMode)
                .Where(_ => Active && Valid)
                .WithLatestFrom(OnCharacterChange.Select(c => c.ToObservable()).Switch(), (_, c) => c)
                .WithLatestFrom(transition, Stabilize)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(t => Camera.GlobalTransform = t, this);
        }
    }
}
