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
using LanguageExt;
using static LanguageExt.Prelude;
using Array = Godot.Collections.Array;

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

        public override bool Valid => base.Valid && Character.IsSome && Camera.IsCurrent();

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Node(false)]
        public virtual Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character;

        public virtual Camera Camera => _camera.IfNone(GetViewport().GetCamera());

        public bool AutoActivate => true;

        [Export]
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
            set => _transitionTime = Mathf.Min(value, 0);
        }

        public float VelocityThreshold
        {
            get => _velocityThreshold;
            set => _velocityThreshold = Mathf.Min(value, 0);
        }

        [Export]
        public float Offset { get; set; }

        public Option<IEntity> FocusedObject => _focus.Bind(f => f.Value);

        public IObservable<Option<IEntity>> OnFocusChange =>
            _focus.MatchObservable(identity, Observable.Empty<Option<IEntity>>);

        public float MaxFocalDistance
        {
            get => _maxFocalDistance;
            set => _maxFocalDistance = Mathf.Min(value, 0);
        }

        public float MaxDofDistance
        {
            get => _maxDofDistance;
            set => _maxDofDistance = Mathf.Min(value, 0);
        }

        public float FocusRange
        {
            get => _focusRange;
            set => _focusRange = Mathf.Min(value, 0);
        }

        public float FocusSpeed
        {
            get => _focusSpeed;
            set => _focusSpeed = Mathf.Min(value, 0);
        }

        public Option<IVision> Vision => Character.Map<IVision>(c => c.Vision);

        public Vector3 Viewpoint => Vision.Map(v => v.Viewpoint).IfNone(Vector3.Zero);

        public Vector3 LookDirection => Vision.Map(v => v.LookDirection).IfNone(Vector3.Forward);

        public override Vector3 Origin => Vision.Map(v => v.Origin).IfNone(Vector3.Zero);

        public override Vector3 Up => Vision.Map(v => v.Up).IfNone(Vector3.Up);

        public override Vector3 Forward => Vision.Map(v => v.Forward).IfNone(Vector3.Forward);

        public override Range<float> YawRange => Vision.Map(v => v.YawRange).IfNone(base.YawRange);

        public override Range<float> PitchRange => Vision.Map(v => v.PitchRange).IfNone(base.PitchRange);

        protected virtual IObservable<Vector2> RotationInput => _rotationInput
            .Bind(i => i.AsVector2Input())
            .MatchObservable(identity, Observable.Empty<Vector2>)
            .Where(_ => Valid);

        protected virtual IObservable<bool> DeactivateInput => _deactivateInput
            .Bind(i => i.FindTrigger())
            .MatchObservable(identity, Observable.Empty<bool>)
            .Where(_ => Valid);

        [Export(PropertyHint.ExpRange, "0.1,5")]
        private float _transitionTime = 2f;

        [Export(PropertyHint.ExpRange, "0.1,5")]
        private float _velocityThreshold = 0.2f;

        [Export(PropertyHint.ExpRange, "1,10")]
        private float _maxFocalDistance = 2f;

        [Export(PropertyHint.ExpRange, "1,10")]
        private float _maxDofDistance = 5f;

        [Export(PropertyHint.ExpRange, "1,10")]
        private float _focusRange = 3f;

        [Export(PropertyHint.ExpRange, "10,1000")]
        private float _focusSpeed = 100f;

        [Export(PropertyHint.ExpRange, "0,1")] private float _minStabilization = 0.2f;

        [Export(PropertyHint.ExpRange, "0,1")] private float _maxStabilization = 0.8f;

        [Export] private NodePath _characterPath;

        [Export] private NodePath _cameraPath;

        [Node("Rotation", false)] private Option<InputBindings> _rotationInput;

        [Node("Deactivate", false)] private Option<InputBindings> _deactivateInput;

        [Node(false)] private Option<Camera> _camera;

        private readonly ReactiveProperty<Option<IHumanoid>> _character;

        private Option<ReactiveProperty<Option<IEntity>>> _focus;

        public HeadMountedView() : this(new Range<float>(-90f, 90f), new Range<float>(-80f, 70f))
        {
        }

        public HeadMountedView(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
            _character = new ReactiveProperty<Option<IHumanoid>>(None).AddTo(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            InitializeInput();
            InitializeStabilization();
            InitializeRaycast();
        }

        private void InitializeInput()
        {
            OnActiveStateChange
                .Do(v => _rotationInput.Iter(i => i.Active = v))
                .Do(v => _deactivateInput.Iter(i => i.Active = v))
                .Subscribe()
                .AddTo(this);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Do(_ => Vision.Iter(v => v.Reset()))
                .Subscribe()
                .AddTo(this);

            RotationInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            OnRotationChange
                .Merge(OnActiveStateChange.Where(identity).Select(_ => Rotation))
                .Subscribe(r => Vision.Iter(v => v.Rotate(r)))
                .AddTo(this);

            DeactivateInput
                .Subscribe(_ => this.Deactivate())
                .AddTo(this);
        }

        private void InitializeStabilization()
        {
            bool IsStablizationAllowed() =>
                Stabilization == StabilizeMode.Always || Stabilization != StabilizeMode.Never;

            Basis GetCharacterRotation() => Character.Select(c => c.GlobalTransform().basis).IfNone(Basis.Identity);

            Quat GetUnstablizedQuat() => (this.GetTransform().basis * this.GetBasis()).Quat();
            Quat GetStablizedQuat() => (GetCharacterRotation() * this.GetBasis()).Quat();

            var movingStateChange = _character
                .Where(c => c.IsSome)
                .SelectMany(c => c.First().Locomotion.OnVelocityChange)
                .Select(v => v.Length() >= VelocityThreshold)
                .DistinctUntilChanged();

            var shouldStablize = movingStateChange
                .Select(v => v && IsStablizationAllowed());

            var transition = this.OnLoop(ProcessMode)
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

            this.OnLoop(ProcessMode)
                .Where(_ => Active && Valid)
                .Zip(cameraTransform.MostRecent(this.GetTransform()), (_, transform) => transform)
                .Subscribe(transform => Camera.SetGlobalTransform(transform))
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
                .Select(to => Character
                    .Map(c => new Array {c})
                    .Bind(v => Camera.GetWorld().IntersectRay(Origin, to, v)));

            onRayCast
                .Select(hit => hit.Select(h => Viewpoint.DistanceTo(h.Position)).IfNone(float.MaxValue))
                .Buffer(
                    TimeSpan.FromMilliseconds(FocusSpeed),
                    TimeSpan.FromMilliseconds(10),
                    this.GetPhysicsScheduler())
                .Where(v => v.Any() && Active)
                .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count)
                .Subscribe(this.SetFocalDistance)
                .AddTo(this);

            _focus = onRayCast
                .Select(hit => hit.Where(h => Viewpoint.DistanceTo(h.Position) <= MaxFocalDistance))
                .Select(hit => hit.Bind(h => h.Collider.FindEntity()))
                .Select(entity => entity.Where(e => e.Valid && e.Visible))
                .DistinctUntilChanged()
                .ToReactiveProperty()
                .AddTo(this);
        }
    }
}
