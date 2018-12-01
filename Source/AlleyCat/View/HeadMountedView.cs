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

        public override bool Valid => base.Valid && Character.IsSome && Camera.IsCurrent();

        public bool AutoActivate => true;

        public enum StabilizeMode
        {
            Never,
            WhileMoving,
            Always
        }

        public StabilizeMode Stabilization { get; set; } = StabilizeMode.WhileMoving;

        public Range<float> StabilizationFactor => new Range<float>(_minStabilization, _maxStabilization);

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
            set => _velocityThreshold = Mathf.Min(value, 0);
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

        public Option<IVision> Vision => Character.Map<IVision>(c => c.Vision);

        public Vector3 Viewpoint => Vision.Map(v => v.Viewpoint).IfNone(Vector3.Zero);

        public Vector3 LookDirection => Vision.Map(v => v.LookDirection).IfNone(Vector3.Forward);

        public override Vector3 Origin => Vision.Map(v => v.Origin).IfNone(Vector3.Zero);

        public override Vector3 Up => Vision.Map(v => v.Up).IfNone(Vector3.Up);

        public override Vector3 Forward => Vision.Map(v => v.Forward).IfNone(Vector3.Forward);

        public override Range<float> YawRange => Vision.Map(v => v.YawRange).IfNone(() => base.YawRange);

        public override Range<float> PitchRange => Vision.Map(v => v.PitchRange).IfNone(() => base.PitchRange);

        public float Offset { get; set; }

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        protected virtual IObservable<Vector2> RotationInput { get; }

        protected virtual IObservable<bool> DeactivateInput { get; }

        private Option<IInputBindings> _rotationInput;

        private Option<IInputBindings> _deactivateInput;

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
                .Where(_ => Valid);
            DeactivateInput = deactivateInput.Bind(i => i.FindTrigger().HeadOrNone())
                .MatchObservable(identity, Observable.Empty<bool>)
                .Where(_ => Valid);

            var onRayCast = TimeSource.OnPhysicsProcess
                .Where(_ => Active && Valid)
                .Select(_ => Viewpoint + LookDirection * Mathf.Max(MaxFocalDistance, MaxDofDistance))
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
                .Subscribe(this.SetFocalDistance, this);

            OnFocusChange = onRayCast
                .Select(hit => hit.Where(h => Viewpoint.DistanceTo(h.GetPosition()) <= MaxFocalDistance))
                .Select(hit => hit.Bind(h => h.GetCollider().FindEntity()))
                .Select(entity => entity.Where(e => e.Valid && e.Visible))
                .DistinctUntilChanged();

            OnFocusChange
                .Do(v => this.LogDebug("Focusing on '{}'.", v))
                .Subscribe(current => FocusedObject = current, this);

            _character = new BehaviorSubject<Option<IHumanoid>>(character).DisposeWith(this);

            _rotationInput = rotationInput;
            _deactivateInput = deactivateInput;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnActiveStateChange.Subscribe(HandleActiveStateChange, this);

            InitializeInput();
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
                Vision.Iter(v => v.Reset());
            }
        }

        private void InitializeInput()
        {
            RotationInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v, this);

            OnRotationChange
                .Merge(OnActiveStateChange.Where(identity).Select(_ => Rotation))
                .Subscribe(r => Vision.Iter(v => v.Rotate(r)), this);

            DeactivateInput
                .Subscribe(_ => this.Deactivate(), this);
        }

        private void InitializeStabilization()
        {
            bool IsStablizationAllowed() =>
                Stabilization == StabilizeMode.Always || Stabilization != StabilizeMode.Never;

            Basis GetCharacterRotation() => Character.Select(c => c.GetGlobalTransform().basis).IfNone(Basis.Identity);

            Quat GetUnstablizedQuat() => (this.GetTransform().basis * this.GetBasis()).Quat();
            Quat GetStablizedQuat() => (GetCharacterRotation() * this.GetBasis()).Quat();

            var movingStateChange = _character
                .Where(c => c.IsSome)
                .SelectMany(c => c.First().Locomotion.OnVelocityChange)
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

            var rotation = Observable.Merge(
                transition
                    .Where(v => v <= 0)
                    .Select(_ => GetUnstablizedQuat()),
                transition
                    .Where(v => v >= 1)
                    .Select(_ => GetStablizedQuat()),
                transition
                    .Select(ratio => GetUnstablizedQuat().Slerp(GetStablizedQuat(), ratio)));

            var cameraTransform = rotation
                .Select(basis => new Transform(basis, Viewpoint + LookDirection * Offset));

            TimeSource.OnProcess(ProcessMode)
                .Where(_ => Active && Valid)
                .Zip(cameraTransform.MostRecent(this.GetTransform()), (_, transform) => transform)
                .Subscribe(transform => Camera.SetGlobalTransform(transform), this);
        }
    }
}
