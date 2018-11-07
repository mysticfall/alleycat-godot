using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Motion;
using AlleyCat.View;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class PlayerControl : GameObject, IPlayerControl
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public override bool Valid => base.Valid && Character.IsSome && Camera.IsCurrent();

        public Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        public Camera Camera { get; }

        public IEnumerable<IPerspectiveView> Perspectives { get; }

        public Option<IPerspectiveView> Perspective
        {
            get => _perspective.Value;
            set => _perspective.OnNext(value);
        }

        public IObservable<Option<IPerspectiveView>> OnPerspectiveChange => _perspective.AsObservable();

        public float MaxFocalDistance
        {
            get => Perspective.OfType<IFocusTracker>().Map(p => p.MaxFocalDistance).HeadOrNone().IfNone(0f);
            set => Perspective.OfType<IFocusTracker>().Iter(p => p.MaxFocalDistance = value);
        }

        public Option<IEntity> FocusedObject =>
            Perspective.OfType<IFocusTracker>().Bind(p => p.FocusedObject).HeadOrNone();

        public IObservable<Option<IEntity>> OnFocusChange =>
            OnPerspectiveChange
                .Select(p => p.OfType<IFocusTracker>().HeadOrNone())
                .SelectMany(p => p.MatchObservable(f => f.OnFocusChange, () => None));

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        protected IObservable<Vector2> MovementInput { get; }

        private Option<InputBindings> _movementInput;

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        private readonly BehaviorSubject<Option<IPerspectiveView>> _perspective;

        private readonly BehaviorSubject<bool> _active;

        private Option<IPerspectiveView> _lastPerspective;

        public PlayerControl(
            Camera camera,
            Option<IHumanoid> character,
            IEnumerable<IPerspectiveView> perspectives,
            Option<InputBindings> movementInput,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active = true)
        {
            Ensure.That(camera, nameof(camera)).IsNotNull();
            Ensure.That(perspectives, nameof(perspectives)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Camera = camera;
            Perspectives = perspectives.Freeze();
            ProcessMode = processMode;
            TimeSource = timeSource;

            MovementInput = movementInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Where(_ => Valid);

            _active = new BehaviorSubject<bool>(active).AddTo(this);
            _character = new BehaviorSubject<Option<IHumanoid>>(character).AddTo(this);
            _perspective = new BehaviorSubject<Option<IPerspectiveView>>(None).AddTo(this);

            _movementInput = movementInput;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Input.SetMouseMode(Input.MouseMode.Captured);

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

            var tick = TimeSource.OnProcess(ProcessMode);

            // TODO: Workaround for smooth view rotation until we add max velocity and acceleration to ILocomotion.
            var viewRotationSpeed = rotatableViews
                .CombineLatest(linearSpeed, tick, (view, speed, delta) =>
                    view.Map(v => v.Yaw).Match(yaw =>
                        {
                            var angularSpeed = Mathf.Min(Mathf.Deg2Rad(120), Mathf.Abs(yaw) * 3) *
                                               Mathf.Sign(yaw) * speed;

                            return Mathf.Abs(angularSpeed * delta) < Mathf.Abs(yaw) ? angularSpeed : yaw / delta;
                        },
                        () => 0
                    ));

            var offsetAngle = viewRotationSpeed.CombineLatest(tick, (speed, delta) => speed * delta);

            locomotion
                .SelectMany(l => l.MatchObservable(v => v.OnProcess(ProcessMode), Observable.Empty<float>))
                .Zip(
                    rotatableViews
                        .CombineLatest(offsetAngle, (view, angle) => (view, angle))
                        .MostRecent((null, 0)),
                    (_, args) => args)
                .Where(_ => Active && Valid)
                .Subscribe(t => t.view.Iter(v => v.Yaw -= t.angle))
                .AddTo(this);

            tick
                .Where(_ => Active && Valid)
                .Zip(viewRotationSpeed.MostRecent(0), (_, speed) => speed)
                .Select(speed => Character.Map(c => c.GetGlobalTransform().Up() * speed).IfNone(Vector3.Zero))
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
