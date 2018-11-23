using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.Physics;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    public class FreeCameraView : TurretLike, IPerspectiveView, IAutoFocusingView
    {
        public override bool Valid => base.Valid && Character.IsSome && Camera.IsCurrent();

        public Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        public Camera Camera { get; }

        public bool AutoActivate => false;

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

        public override Vector3 Origin => Camera.GlobalTransform.origin;

        public override Vector3 Forward => Camera.GlobalTransform.Forward();

        public override Vector3 Up => Vector3.Up;

        protected virtual IObservable<Vector2> RotationInput { get; }

        protected virtual IObservable<Vector2> MovementInput { get; }

        protected virtual IObservable<bool> ToggleInput { get; }

        protected ITimeSource TimeSource { get; }

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        private Option<IInputBindings> _rotationInput;

        private Option<IInputBindings> _movementInput;

        private float _maxDofDistance = 5f;

        private float _focusRange = 3f;

        private float _focusSpeed = 100f;

        public FreeCameraView(
            Camera camera,
            Option<IHumanoid> character,
            Option<IInputBindings> rotationInput,
            Option<IInputBindings> movementInput,
            Option<IInputBindings> toggleInput,
            Range<float> yawRange,
            Range<float> pitchRange,
            ITimeSource timeSource,
            bool active,
            ILogger logger) : base(yawRange, pitchRange, active, logger)
        {
            Ensure.That(camera, nameof(camera)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Camera = camera;

            RotationInput = rotationInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Where(_ => Valid);
            MovementInput = movementInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Where(_ => Valid);
            ToggleInput = toggleInput.Bind(i => i.FindTrigger().HeadOrNone())
                .MatchObservable(identity, Observable.Empty<bool>)
                .Where(_ => Valid);

            TimeSource = timeSource;

            _character = new BehaviorSubject<Option<IHumanoid>>(character).AddTo(this);

            _rotationInput = rotationInput;
            _movementInput = movementInput;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnActiveStateChange
                .Where(_ => Valid)
                .Subscribe(v => Character.Iter(c => c.Locomotion.Active = !v))
                .AddTo(this);

            InitializeInput();
            InitializeRaycast();
        }

        private void InitializeInput()
        {
            OnActiveStateChange
                .Do(v => _rotationInput.Iter(i => i.Active = v))
                .Do(v => _movementInput.Iter(i => i.Active = v))
                .Subscribe()
                .AddTo(this);

            RotationInput
                .Select(v => v * 0.1f)
                .Do(v => Camera.GlobalRotate(new Vector3(0, 1, 0), -v.x))
                .Do(v => Camera.RotateObjectLocal(new Vector3(1, 0, 0), -v.y))
                .Subscribe()
                .AddTo(this);

            MovementInput
                .Select(v => new Vector3(v.x, 0, -v.y) * 0.02f)
                .Subscribe(Camera.TranslateObjectLocal)
                .AddTo(this);

            ToggleInput
                .Subscribe(_ => Active = !Active)
                .AddTo(this);
        }

        private void InitializeRaycast()
        {
            OnActiveStateChange
                .Where(s => s)
                .Subscribe(_ => this.EnableDof())
                .AddTo(this);

            TimeSource.OnPhysicsProcess
                .Where(_ => Active && Valid)
                .Select(_ => Origin + Forward * MaxDofDistance)
                .Select(to => Camera.GetWorld().IntersectRay(Origin, to))
                .Select(hit => hit.Select(h => Origin.DistanceTo(h.Position)).IfNone(float.MaxValue))
                .Buffer(
                    TimeSpan.FromMilliseconds(FocusSpeed),
                    TimeSpan.FromMilliseconds(10),
                    TimeSource.PhysicsScheduler)
                .Where(v => v.Any() && Active)
                .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count)
                .Subscribe(this.SetFocalDistance)
                .AddTo(this);
        }
    }
}
