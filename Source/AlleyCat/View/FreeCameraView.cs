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
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    [Singleton(typeof(IPerspectiveView))]
    public class FreeCameraView : TurretLike, IPerspectiveView, IAutoFocusingView
    {
        public override bool Valid => base.Valid && Character != null && Camera != null && Camera.IsCurrent();

        public virtual IHumanoid Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<IHumanoid> OnCharacterChange => _character;

        [Node(required: false)]
        public virtual Camera Camera { get; private set; }

        public bool AutoActivate => false;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxDofDistance { get; set; } = 5f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float FocusRange { get; set; } = 3f;

        [Export(PropertyHint.ExpRange, "10,1000")]
        public float FocusSpeed { get; set; } = 100f;

        public override Vector3 Origin => Camera?.GlobalTransform.origin ?? Vector3.Zero;

        public override Vector3 Forward => Camera?.GlobalTransform.Forward() ?? Vector3.Forward;

        public override Vector3 Up => Vector3.Up;

        protected IObservable<Vector2> RotationInput => _rotationInput.AsVector2Input().Where(_ => Valid);

        protected IObservable<Vector2> MovementInput => _movementInput.AsVector2Input().Where(_ => Valid);

        [CanBeNull]
        protected virtual IObservable<bool> ToggleInput => _toggleInput.GetTrigger().Where(_ => Valid);

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Rotation")] private InputBindings _rotationInput;

        [Node("Movement")] private InputBindings _movementInput;

        [Node("Toggle", false)] private InputBindings _toggleInput;

        private readonly ReactiveProperty<IHumanoid> _character = new ReactiveProperty<IHumanoid>();

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Camera = Camera ?? GetViewport().GetCamera();

            OnActiveStateChange
                .Where(_ => Valid)
                // ReSharper disable once PossibleNullReferenceException
                .Subscribe(v => Character.Locomotion.Active = !v)
                .AddTo(this);

            InitializeInput();
            InitializeRaycast();
        }

        private void InitializeInput()
        {
            OnActiveStateChange
                .Do(v => _rotationInput.Active = v)
                .Do(v => _movementInput.Active = v)
                .Subscribe()
                .AddTo(this);

            RotationInput
                .Select(v => v * 0.1f)
                .Do(v => Camera?.GlobalRotate(new Vector3(0, 1, 0), -v.x))
                .Do(v => Camera?.RotateObjectLocal(new Vector3(1, 0, 0), -v.y))
                .Subscribe()
                .AddTo(this);

            MovementInput
                .Select(v => new Vector3(v.x, 0, -v.y) * 0.02f)
                .Subscribe(v => Camera?.TranslateObjectLocal(v))
                .AddTo(this);

            ToggleInput?
                .Subscribe(_ => Active = !Active)
                .AddTo(this);
        }

        private void InitializeRaycast()
        {
            OnActiveStateChange
                .Where(s => s)
                .Subscribe(_ => this.EnableDof())
                .AddTo(this);

            this.OnPhysicsProcess()
                .Where(_ => Active && Valid)
                .Select(_ => Origin + Forward * MaxDofDistance)
                .Select(to => Camera.GetWorld().IntersectRay(Origin, to))
                .Select(hit => hit == null ? float.MaxValue : Origin.DistanceTo(hit.Position))
                .Buffer(
                    TimeSpan.FromMilliseconds(FocusSpeed),
                    TimeSpan.FromMilliseconds(10),
                    this.GetPhysicsScheduler())
                .Where(v => v.Any() && Active)
                .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count)
                .Subscribe(this.SetFocalDistance)
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _character?.Dispose();

            base.Dispose(disposing);
        }
    }
}
