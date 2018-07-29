using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.Physics;
using AlleyCat.Sensor;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    [Singleton(typeof(IPerspectiveView), typeof(IFirstPersonView))]
    public class HeadMountedView : TurretLike, IFirstPersonView, IAutoFocusingView
    {
        public enum StabilizeMode
        {
            Never,
            WhileMoving,
            Always
        }

        public override bool Valid => base.Valid && Character != null && Camera != null && Camera.IsCurrent();

        public virtual IHumanoid Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<IHumanoid> OnCharacterChange => _character;

        [Node(required: false)]
        public virtual Camera Camera { get; private set; }

        public bool AutoActivate => true;

        [Export]
        public StabilizeMode Stabilization { get; set; } = StabilizeMode.WhileMoving;

        public Range<float> StabilizationFactor => new Range<float>(_minStabilization, _maxStabilization);

        [Export(PropertyHint.ExpRange, "0.1,5")]
        public float TransitionTime { get; set; } = 2f;

        [Export(PropertyHint.ExpRange, "0.1,5")]
        public float VelocityThreshold { get; set; } = 0.2f;

        [Export]
        public float Offset { get; set; }

        public IEntity FocusedObject => _focus?.Value;

        public IObservable<IEntity> OnFocusChange => _focus ?? Observable.Empty<IEntity>();

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxFocalDistance { get; set; } = 2f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxDofDistance { get; set; } = 5f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float FocusRange { get; set; } = 3f;

        [Export(PropertyHint.ExpRange, "10,1000")]
        public float FocusSpeed { get; set; } = 100f;

        [CanBeNull]
        public IVision Vision => Character?.Vision;

        public Vector3 Viewpoint => Vision?.Viewpoint ?? Vector3.Zero;

        public Vector3 LookDirection => Vision?.LookDirection ?? Forward;

        public override Vector3 Origin => Vision?.Origin ?? Vector3.Zero;

        public override Vector3 Up => Vision?.Up ?? Vector3.Up;

        public override Vector3 Forward => Vision?.Forward ?? Vector3.Forward;

        public override Range<float> YawRange => Vision?.YawRange ?? base.YawRange;

        public override Range<float> PitchRange => Vision?.PitchRange ?? base.PitchRange;

        protected virtual IObservable<Vector2> RotationInput => _rotationInput.AsVector2Input().Where(_ => Valid);

        [CanBeNull]
        protected virtual IObservable<bool> DeactivateInput => _deactivateInput.GetTrigger().Where(_ => Valid);

        [Export(PropertyHint.ExpRange, "0,1")] private float _minStabilization = 0.2f;

        [Export(PropertyHint.ExpRange, "0,1")] private float _maxStabilization = 0.8f;

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Rotation")] private InputBindings _rotationInput;

        [Node("Deactivate", false)] private InputBindings _deactivateInput;

        private readonly ReactiveProperty<IHumanoid> _character = new ReactiveProperty<IHumanoid>();

        private ReactiveProperty<IEntity> _focus;

        public HeadMountedView() : base(new Range<float>(-90f, 90f), new Range<float>(-80f, 70f))
        {
            ProcessMode = ProcessMode.Idle;
        }

        public HeadMountedView(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Camera = Camera ?? GetViewport().GetCamera();

            InitializeInput();
            InitializeStabilization();
            InitializeRaycast();
        }

        private void InitializeInput()
        {
            OnActiveStateChange
                .Subscribe(v => _rotationInput.Active = v)
                .AddTo(this);

            OnActiveStateChange
                .Where(_ => _deactivateInput != null)
                .Subscribe(v => _deactivateInput.Active = v)
                .AddTo(this);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Do(_ => Vision?.Reset())
                .Subscribe()
                .AddTo(this);

            RotationInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            OnRotationChange
                .Merge(OnActiveStateChange.Where(v => v).Select(_ => Rotation))
                .Subscribe(v => Vision?.Rotate(v))
                .AddTo(this);

            DeactivateInput?
                .Subscribe(_ => this.Deactivate())
                .AddTo(this);
        }

        private void InitializeStabilization()
        {
            bool IsStablizationAllowed() =>
                Stabilization == StabilizeMode.Always || Stabilization != StabilizeMode.Never;

            Basis GetCharacterRotation() => Character?.GlobalTransform().basis ?? Basis.Identity;

            Quat GetUnstablizedQuat() => (this.GetTransform().basis * this.GetBasis()).Quat();
            Quat GetStablizedQuat() => (GetCharacterRotation() * this.GetBasis()).Quat();

            var movingStateChange = _character
                .Where(c => c != null)
                .SelectMany(c => c.Locomotion.OnVelocityChange)
                .Select(v => v.Length() >= VelocityThreshold)
                .DistinctUntilChanged();

            var shouldStablize = movingStateChange
                .Select(v => v && IsStablizationAllowed());

            var transition = OnLoop
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

            OnLoop
                .Where(_ => Active && Valid)
                .Zip(cameraTransform.MostRecent(this.GetTransform()), (_, transform) => transform)
                .Subscribe(transform => Camera?.SetGlobalTransform(transform))
                .AddTo(this);
        }

        private void InitializeRaycast()
        {
            OnActiveStateChange
                .Where(s => s)
                .Subscribe(_ => this.EnableDof())
                .AddTo(this);

            var onRayCast = this.OnPhysicsProcess()
                .Where(_ => Active && Valid)
                .Select(_ => Viewpoint + LookDirection * Mathf.Max(MaxFocalDistance, MaxDofDistance))
                .Select(to => Camera.GetWorld().IntersectRay(Viewpoint, to, new object[] {Character}));

            onRayCast
                .Select(hit => hit == null ? float.MaxValue : Viewpoint.DistanceTo(hit.Position))
                .Buffer(
                    TimeSpan.FromMilliseconds(FocusSpeed),
                    TimeSpan.FromMilliseconds(10),
                    this.GetPhysicsScheduler())
                .Where(v => v.Any() && Active)
                .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count)
                .Subscribe(this.SetFocalDistance)
                .AddTo(this);

            _focus = onRayCast
                .Where(hit => hit == null || Viewpoint.DistanceTo(hit.Position) <= MaxFocalDistance)
                .Select(hit => hit?.Collider?.FindEntity())
                .Select(e => e != null && e.Valid && e.Visible ? e : null)
                .ToReactiveProperty();
        }

        protected override void Dispose(bool disposing)
        {
            _focus?.Dispose();
            _character?.Dispose();

            base.Dispose(disposing);
        }
    }
}
