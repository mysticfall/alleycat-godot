using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    [Singleton(typeof(IPerspectiveView), typeof(IFirstPersonView))]
    public class HeadMountedView : TurretLike, IFirstPersonView
    {
        public enum StabilizeMode
        {
            Never,
            WhileMoving,
            Always
        }

        public override bool Valid => base.Valid && Character != null && Camera != null && Camera.IsCurrent();

        [Node(required: false)]
        public virtual IHumanoid Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        [Node(required: false)]
        public virtual Camera Camera { get; set; }

        public bool AutoActivate => true;

        [Export]
        public StabilizeMode Stabilization { get; set; } = StabilizeMode.WhileMoving;

        [Export(PropertyHint.ExpRange, "0,1")]
        public float StabilizationFactor { get; set; } = 0.8f;

        [Export(PropertyHint.ExpRange, "0.1,5")]
        public float TransitionTime { get; set; } = 2f;

        [Export(PropertyHint.ExpRange, "0.1,5")]
        public float VelocityThreshold { get; set; } = 0.2f;

        [Export]
        public float Offset { get; set; }

        [CanBeNull]
        public IVision Vision => Character?.Vision;

        public Vector3 Viewpoint => (Vision?.Viewpoint ?? Vector3.Zero) + Forward * Offset;

        public override Vector3 Origin => Vision?.Origin ?? Vector3.Zero;

        public override Vector3 Up => Vision?.Up ?? Vector3.Up;

        public override Vector3 Forward => Vision?.Forward ?? Vector3.Forward;

        public override Range<float> YawRange => Vision?.YawRange ?? base.YawRange;

        public override Range<float> PitchRange => Vision?.PitchRange ?? base.PitchRange;

        protected virtual IObservable<Vector2> ViewInput => _viewInput.AsVector2Input().Where(_ => Active && Valid);

        [CanBeNull]
        protected virtual IObservable<bool> DeactivateInput =>
            _deactivateInput.GetTrigger().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Rotation")] private InputBindings _viewInput;

        [Node("Deactivate", false)] private InputBindings _deactivateInput;

        private readonly ReactiveProperty<IHumanoid> _character = new ReactiveProperty<IHumanoid>();

        public HeadMountedView() : base(new Range<float>(-90f, 90f), new Range<float>(-80f, 70f))
        {
            ProcessMode = ProcessMode.Idle;
        }

        public HeadMountedView(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            ViewInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            OnRotationChange
                .Subscribe(v => Vision?.Rotate(v))
                .AddTo(this);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Do(_ => this.Reset())
                .Do(_ => Vision?.Reset())
                .Subscribe()
                .AddTo(this);

            DeactivateInput?
                .Subscribe(_ => this.Deactivate())
                .AddTo(this);

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
                .Select(ratio => Mathf.Max(ratio, StabilizationFactor));

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
                .Select(basis => new Transform(basis, Viewpoint))
                .ToReactiveProperty();

            OnLoop
                .Where(_ => Active && Valid)
                .Zip(cameraTransform.MostRecent(cameraTransform.Value), (_, transform) => transform)
                .Subscribe(transform => Camera?.SetGlobalTransform(transform))
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _character?.Dispose();

            base.Dispose(disposing);
        }
    }
}
