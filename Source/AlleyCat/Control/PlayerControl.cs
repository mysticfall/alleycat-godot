using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.View;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    [AutowireContext, Singleton(typeof(IPlayerControl), typeof(IPerspectiveSwitcher), typeof(IFocusTracker))]
    public class PlayerControl : AutowiredNode, IPlayerControl
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        public override bool Valid => base.Valid &&
                                      _movementInput.IsSome &&
                                      Character.IsSome &&
                                      Perspective.IsSome &&
                                      Camera.IsCurrent();

        [Node(false)]
        public virtual Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character;

        public Camera Camera
        {
            get => _camera.IfNone(GetViewport().GetCamera);
            set
            {
                Ensure.That(value, nameof(value)).IsNotNull();

                _camera = Some(value);
            }
        }

        [Service]
        public IEnumerable<IPerspectiveView> Perspectives { get; private set; } =
            Enumerable.Empty<IPerspectiveView>();

        public Option<IPerspectiveView> Perspective
        {
            get => _perspective.Value;
            set => _perspective.Value = value;
        }

        public IObservable<Option<IPerspectiveView>> OnPerspectiveChange =>
            _perspective.Where(v => Active && Valid);

        [Export(PropertyHint.ExpRange, "0, 100, 5")]
        public float MaxFocalDistance
        {
            get => _maxFocalDistance;
            set => _maxFocalDistance = Mathf.Min(value, 0);
        }

        public Option<IEntity> FocusedObject =>
            Perspective.OfType<IFocusTracker>().Bind(p => p.FocusedObject).HeadOrNone();

        public IObservable<Option<IEntity>> OnFocusChange =>
            OnPerspectiveChange
                .Select(p => p.OfType<IFocusTracker>().HeadOrNone())
                .SelectMany(p => p.MatchObservable(f => f.OnFocusChange, () => None));

        protected IObservable<Vector2> MovementInput =>
            _movementInput.Bind(i => i.AsVector2Input()).Map(i => i.Where(_ => Valid)).Head();

        [Export] private NodePath _characterPath;

        [Export] private NodePath _cameraPath;

        [Node("Movement")] private Option<InputBindings> _movementInput;

        private readonly ReactiveProperty<Option<IHumanoid>> _character;

        private readonly ReactiveProperty<Option<IPerspectiveView>> _perspective;

        private readonly ReactiveProperty<bool> _active;

        [Node(false)] private Option<Camera> _camera;

        private Option<IPerspectiveView> _lastPerspective;

        private float _maxFocalDistance = 5f;

        public PlayerControl()
        {
            _active = new ReactiveProperty<bool>(true).AddTo(this);
            _character = new ReactiveProperty<Option<IHumanoid>>(None).AddTo(this);
            _perspective = new ReactiveProperty<Option<IPerspectiveView>>(None).AddTo(this);
        }

        [PostConstruct]
        protected void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Character |= this.FindPlayer<IHumanoid>();

            Option<IPerspectiveView> active = None;

            foreach (var perspective in Perspectives)
            {
                perspective.Character = Character;

                active |= Optional(perspective).Where(p => p.Active);

                perspective.OnActiveStateChange
                    .Where(s => Active && !s && Perspective.Contains(perspective))
                    .Subscribe(_ => Perspective = FindNextValidPerspective(Some(perspective)))
                    .AddTo(this);

                perspective.OnActiveStateChange
                    .Where(s => s && !Perspective.Contains(perspective))
                    .Subscribe(_ => Perspective = Some(perspective))
                    .AddTo(this);
            }

            Perspective |= active;

            OnActiveStateChange
                .Do(v => _movementInput.Iter(i => i.Active = v))
                .Do(v => Character.Iter(c => c.Locomotion.Stop()))
                .Do(v => Perspective.Iter(p => p.Active = v))
                .Subscribe()
                .AddTo(this);

            MovementInput
                .Where(_ => Character.Exists(c => c.Valid))
                .Where(_ => Perspective.Exists(p => p.AutoActivate))
                .Select(v => new Vector3(v.x, 0, -v.y) * 2)
                .Subscribe(v => Character.Iter(c => c.Locomotion.Move(v)))
                .AddTo(this);

            OnPerspectiveChange
                .Pairwise()
                .Subscribe(t => OnPerspectiveChanged(t.Item1, t.Item2))
                .AddTo(this);

            var rotatableViews = OnPerspectiveChange.Select(p => p.OfType<ITurretLike>().HeadOrNone());
            var locomotion = _character.Select(c => c.Select(v => v.Locomotion));

            var linearSpeed = locomotion
                .SelectMany(l => l.MatchObservable(v => v.OnVelocityChange, () => Vector3.Zero))
                .Select(v => v.Length());

            // TODO: Workaround for smooth view rotation until we add max velocity and acceleration to ILocomotion.
            var viewRotationSpeed = rotatableViews
                .CombineLatest(linearSpeed, this.OnLoop(ProcessMode), (view, speed, delta) =>
                    view.Map(v => v.Yaw).Match(yaw =>
                        {
                            var angularSpeed = Mathf.Min(Mathf.Deg2Rad(120), Mathf.Abs(yaw) * 3) *
                                               Mathf.Sign(yaw) * speed;

                            return Mathf.Abs(angularSpeed * delta) < Mathf.Abs(yaw) ? angularSpeed : yaw / delta;
                        },
                        () => 0
                    ));

            var offsetAngle = viewRotationSpeed
                .CombineLatest(this.OnLoop(ProcessMode), (speed, delta) => speed * delta);

            locomotion
                .Where(_ => Active && Valid)
                .SelectMany(l => l.MatchObservable(v => v.OnLoop(ProcessMode), Observable.Empty<float>))
                .Zip(
                    rotatableViews
                        .CombineLatest(offsetAngle, (view, angle) => (view, angle))
                        .MostRecent((null, 0)),
                    (_, args) => args)
                .Where(t => t.view != null)
                .Subscribe(t => t.view.Iter(v => v.Yaw -= t.angle))
                .AddTo(this);

            this.OnLoop(ProcessMode)
                .Where(_ => Active && Valid)
                .Zip(viewRotationSpeed.MostRecent(0), (_, speed) => speed)
                .Select(speed => Character.Map(c => c.GlobalTransform().Up() * speed).IfNone(Vector3.Zero))
                .CombineLatest(locomotion, (velocity, loco) => (loco, velocity))
                .Subscribe(t => t.loco.Iter(l => l.Rotate(t.velocity)))
                .AddTo(this);
        }

        protected virtual void OnPerspectiveChanged(
            Option<IPerspectiveView> previous, Option<IPerspectiveView> current)
        {
            (
                from previousRotatable in previous.OfType<ITurretLike>()
                from currentRotatable in current.OfType<ITurretLike>()
                select (previousRotatable, currentRotatable)
            ).Iter(t => t.currentRotatable.Rotation = t.previousRotatable.Rotation);

            if (!current.Exists(c => c is IAutoFocusingView))
            {
                previous.OfType<IAutoFocusingView>().Iter(v => v.DisableDof());
            }

            _lastPerspective = previous.Filter(p => p.AutoActivate);

            current.Iter(c => c.Activate());
            previous.Iter(p => p.Deactivate());
        }

        protected virtual Option<IPerspectiveView> FindNextValidPerspective(Option<IPerspectiveView> current)
        {
            return _lastPerspective
                .Concat(Perspectives)
                .Filter(p => !current.Contains(p) && p.Valid && p.AutoActivate)
                .HeadOrNone();
        }
    }
}
