using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Action;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using AlleyCat.Motion;
using AlleyCat.View;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static AlleyCat.Common.MathUtils;
using static Godot.Mathf;
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

        public Map<string, IAction> Actions { get; }

        public float MaxFocalDistance
        {
            get => FocusTracker.Map(p => p.MaxFocalDistance).IfNone(0f);
            set => FocusTracker.Iter(p => p.MaxFocalDistance = value);
        }

        public Option<IEntity> FocusedObject => FocusTracker.Bind(p => p.FocusedObject);

        public Option<IFocusTracker> FocusTracker => Perspective.OfType<IFocusTracker>().HeadOrNone();

        public IObservable<Option<IEntity>> OnFocusChange =>
            OnPerspectiveChange
                .Select(p => p.OfType<IFocusTracker>().HeadOrNone())
                .SelectMany(p => p.MatchObservable(f => f.OnFocusChange, () => None));

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        protected IObservable<Vector2> MovementInput { get; }

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        private readonly BehaviorSubject<Option<IPerspectiveView>> _perspective;

        private readonly BehaviorSubject<bool> _active;

        private Option<IPerspectiveView> _lastPerspective;

        public PlayerControl(
            Camera camera,
            Option<IHumanoid> character,
            IEnumerable<IPerspectiveView> perspectives,
            IEnumerable<IAction> actions,
            Option<IInputBindings> movementInput,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(camera, nameof(camera)).IsNotNull();
            Ensure.That(perspectives, nameof(perspectives)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Camera = camera;
            Perspectives = perspectives.Freeze();
            Actions = actions.ToMap();
            ProcessMode = processMode;
            TimeSource = timeSource;

            MovementInput = movementInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Where(_ => Valid);

            _active = CreateSubject(active);
            _character = CreateSubject(character);
            _perspective = CreateSubject<Option<IPerspectiveView>>(None);
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
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(_ => Perspective = FindNextValidPerspective(Some(perspective)), this);

                perspective.OnActiveStateChange
                    .Where(s => s && !Perspective.Contains(perspective))
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(_ => Perspective = Some(perspective), this);
            }

            Perspective |= active;

            OnActiveStateChange
                .Do(v => Character.Iter(c => c.Locomotion.Active = v))
                .Do(v => Perspective.Iter(p => p.Active = v))
                .Do(v => Actions.Values.Iter(p => p.Active = v))
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(this);

            OnPerspectiveChange
                .Pairwise()
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(t => OnPerspectiveChanged(t.Item1, t.Item2), this);

            const float movementModifier = 2f;

            var movementInput = MovementInput
                .Where(_ => Character.Exists(c => c.Valid))
                .Where(_ => Perspective.Exists(p => p.AutoActivate));

            var facing = movementInput
                .Select(v => Atan2(v.x, v.y));

            var inputStrength = facing
                .Select(v => Abs(Sin(v)) + Abs(Cos(v)))
                .Select(v => (v - 1) / 0.414214f + 1);

            movementInput
                .CombineLatest(inputStrength, (input, strength) => (input, strength))
                .Select(v => (Abs(v.input.x) + Abs(v.input.y)) / v.strength)
                .Select(v => new Vector3(0, 0, -v) * movementModifier)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Character.Iter(c => c.Locomotion.Move(v)), this);

            var rotatableViews = OnPerspectiveChange.Select(p => p.OfType<ITurretLike>().HeadOrNone());
            var locomotion = _character.Select(c => c.Select(v => v.Locomotion));

            var linearSpeed = locomotion
                .SelectMany(l => l.MatchObservable(v => v.OnVelocityChange, () => Vector3.Zero))
                .Select(v => v.Length());

            var tick = TimeSource.OnProcess(ProcessMode).Where(_ => Active && Valid);

            var viewRotationSpeed = rotatableViews.CombineLatest(linearSpeed, facing, tick,
                (view, speed, offset, delta) =>
                    view.Map(v => v.YawRange.Clamp(offset) - v.Yaw).Map(NormalizeAspectAngle).Match(target =>
                        {
                            // TODO: Workaround for smooth view rotation until we add max velocity and acceleration to ILocomotion.
                            var angularSpeed = Min(Deg2Rad(120), Abs(target) * 3) * Sign(target) * Abs(speed);

                            return Abs(angularSpeed * delta) < Abs(target) ? angularSpeed : target / delta;
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
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(t => t.view.Iter(v => v.Yaw = NormalizeAspectAngle(v.Yaw + t.angle)), this);

            tick
                .Zip(viewRotationSpeed.MostRecent(0), (_, speed) => speed)
                .Select(speed => Character.Map(c => c.GetGlobalTransform().Up() * speed).IfNone(Vector3.Zero))
                .CombineLatest(locomotion, (velocity, loco) => (loco, velocity))
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(t => t.loco.Iter(l => l.Rotate(t.velocity)), this);
        }

        protected virtual void OnPerspectiveChanged(
            Option<IPerspectiveView> previous, Option<IPerspectiveView> current)
        {
            this.LogDebug("Perspective has changed: {} -> {}.", previous, current);

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
