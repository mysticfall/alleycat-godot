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
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    [Singleton(typeof(IPerspectiveView))]
    public class FreeCameraView : TurretLike, IPerspectiveView, IAutoFocusingView
    {
        public override bool Valid => base.Valid && Character.IsSome && Camera.IsCurrent();

        [Node(false)]
        public virtual Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character;

        public virtual Camera Camera => _camera.IfNone(GetViewport().GetCamera());

        public bool AutoActivate => false;

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

        public override Vector3 Origin => Camera.GlobalTransform.origin;

        public override Vector3 Forward => Camera.GlobalTransform.Forward();

        public override Vector3 Up => Vector3.Up;

        protected virtual IObservable<Vector2> RotationInput => _rotationInput
            .Bind(i => i.AsVector2Input())
            .MatchObservable(identity, Observable.Empty<Vector2>)
            .Where(_ => Valid);

        protected virtual IObservable<Vector2> MovementInput => _movementInput
            .Bind(i => i.AsVector2Input())
            .MatchObservable(identity, Observable.Empty<Vector2>)
            .Where(_ => Valid);

        protected virtual IObservable<bool> ToggleInput =>
            _toggleInput.Bind(i => i.FindTrigger().HeadOrNone())
                .MatchObservable(identity, Observable.Empty<bool>)
                .Where(_ => Valid);

        [Export] private NodePath _characterPath;

        [Export] private NodePath _cameraPath;

        [Export(PropertyHint.ExpRange, "1,10")]
        private float _maxDofDistance = 5f;

        [Export(PropertyHint.ExpRange, "1,10")]
        private float _focusRange = 3f;

        [Export(PropertyHint.ExpRange, "10,1000")]
        private float _focusSpeed = 100f;

        [Node("Rotation", false)] private Option<InputBindings> _rotationInput;

        [Node("Movement", false)] private Option<InputBindings> _movementInput;

        [Node("Toggle", false)] private Option<InputBindings> _toggleInput;

        [Node(false)] private Option<Camera> _camera;

        private readonly ReactiveProperty<Option<IHumanoid>> _character;

        public FreeCameraView()
        {
            _character = new ReactiveProperty<Option<IHumanoid>>(None).AddTo(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

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
                .Subscribe(v => Camera.TranslateObjectLocal(v))
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

            this.OnPhysicsProcess()
                .Where(_ => Active && Valid)
                .Select(_ => Origin + Forward * MaxDofDistance)
                .Select(to => Camera.GetWorld().IntersectRay(Origin, to))
                .Select(hit => hit.Select(h => Origin.DistanceTo(h.Position)).IfNone(float.MaxValue))
                .Buffer(
                    TimeSpan.FromMilliseconds(FocusSpeed),
                    TimeSpan.FromMilliseconds(10),
                    this.GetPhysicsScheduler())
                .Where(v => v.Any() && Active)
                .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count)
                .Subscribe(this.SetFocalDistance)
                .AddTo(this);
        }
    }
}
