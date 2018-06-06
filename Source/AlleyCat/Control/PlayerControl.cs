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
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    [AutowireContext, Singleton(typeof(IPlayerControl), typeof(IFocusTracker))]
    public class PlayerControl : AutowiredNode, IPlayerControl
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public virtual bool Valid => Character != null && Camera != null && Camera.IsCurrent();

        [Node(required: false)]
        public virtual IHumanoid Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<IHumanoid> OnCharacterChange => _character;

        [Node(required: false)]
        public Camera Camera { get; set; }

        [Service]
        public IEnumerable<IPerspectiveView> Perspectives { get; private set; }

        public IPerspectiveView Perspective
        {
            get => _perspective.Value;
            set => _perspective.Value = value;
        }

        public IObservable<IPerspectiveView> OnPerspectiveChange => _perspective.Where(v => Active && Valid);

        public float MaxFocalDistance { get; set; } = 5f;

        public IEntity FocusedObject => (Perspective as IFocusTracker)?.FocusedObject;

        public IObservable<IEntity> OnFocusChange
        {
            get
            {
                var supported = OnPerspectiveChange.OfType<IFocusTracker>().SelectMany(t => t.OnFocusChange);
                var unsupported = OnPerspectiveChange.Where(p => !(p is IFocusTracker)).Select(_ => default(IEntity));

                return supported.Merge(unsupported).DistinctUntilChanged();
            }
        }

        protected IObservable<Vector2> MovementInput => _movementInput.AsVector2Input().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Movement")] private InputBindings _movementInput;

        private readonly ReactiveProperty<IHumanoid> _character = new ReactiveProperty<IHumanoid>();

        private readonly ReactiveProperty<IPerspectiveView> _perspective = new ReactiveProperty<IPerspectiveView>();

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private IPerspectiveView _lastPerspective;

        public PlayerControl()
        {
            ProcessMode = ProcessMode.Idle;
        }

        [PostConstruct]
        protected void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Character = Character ?? GetTree().GetNodesInGroup<IHumanoid>(Tags.Player).FirstOrDefault();

            IPerspectiveView active = null;

            foreach (var perspective in Perspectives)
            {
                perspective.Character = Character;

                if (perspective.Active && active == null)
                {
                    active = perspective;
                }

                perspective.OnActiveStateChange
                    .Where(s => !s && perspective == Perspective)
                    .Select(_ => FindNextValidPerspective(perspective))
                    .Where(p => p != null)
                    .Subscribe(p => Perspective = p)
                    .AddTo(this);

                perspective.OnActiveStateChange
                    .Where(s => s && perspective != Perspective)
                    .Subscribe(_ => Perspective = perspective)
                    .AddTo(this);
            }

            if (active != null)
            {
                Perspective = active;
            }

            MovementInput
                .Where(_ => Character?.Locomotion != null)
                .Where(_ => Perspective.AutoActivate)
                .Select(v => new Vector3(v.x, 0, -v.y) * 2)
                .Subscribe(v => Character?.Locomotion.Move(v))
                .AddTo(this);

            OnPerspectiveChange
                .Pairwise()
                .Subscribe(t => OnPerspectiveChanged(t.Item1, t.Item2))
                .AddTo(this);

            var rotatableViews = OnPerspectiveChange.Select(p => p as ITurretLike);
            var locomotion = _character.Where(c => c != null).Select(c => c.Locomotion);

            var linearSpeed = locomotion.SelectMany(l => l.OnVelocityChange).Select(v => v.Length());

            // TODO: Workaround for smooth view rotation until we add max velocity and acceleration to ILocomotion.
            var viewRotationSpeed =
                rotatableViews.CombineLatest(linearSpeed, OnLoop, (view, speed, delta) =>
                {
                    if (view == null) return 0;

                    var angularSpeed = Mathf.Min(Mathf.Deg2Rad(120), Mathf.Abs(view.Yaw) * 3) *
                                       Mathf.Sign(view.Yaw) *
                                       speed;

                    return Mathf.Abs(angularSpeed * delta) < Mathf.Abs(view.Yaw) ? angularSpeed : view.Yaw / delta;
                });

            var offsetAngle =
                viewRotationSpeed.CombineLatest(OnLoop, (speed, delta) => speed * delta);

            locomotion
                .Where(_ => Active && Valid)
                .SelectMany(l => l.OnLoop)
                .Zip(
                    rotatableViews
                        .CombineLatest(offsetAngle, (view, angle) => (view, angle))
                        .MostRecent((null, 0)),
                    (_, args) => args)
                .Where(t => t.Item1 != null)
                .Subscribe(t => t.Item1.Yaw -= t.Item2)
                .AddTo(this);

            OnLoop
                .Where(_ => Active && Valid)
                .Zip(viewRotationSpeed.MostRecent(0), (_, speed) => speed)
                .Select(speed => Character?.GlobalTransform().Up() * speed ?? Vector3.Zero)
                .CombineLatest(locomotion, (velocity, loco) => (loco, velocity))
                .Subscribe(t => t.Item1.Rotate(t.Item2))
                .AddTo(this);
        }

        protected virtual void OnPerspectiveChanged(IPerspectiveView previous, IPerspectiveView current)
        {
            if (previous is ITurretLike previousRotatable && current is ITurretLike currentRotatable)
            {
                currentRotatable.Rotation = previousRotatable.Rotation;
            }

            if (previous is IAutoFocusingView focusView && !(current is IAutoFocusingView))
            {
                focusView.DisableDof();
            }

            _lastPerspective = previous != null && previous.AutoActivate ? previous : null;

            current?.Activate();
            previous?.Deactivate();
        }

        protected virtual IPerspectiveView FindNextValidPerspective([CanBeNull] IPerspectiveView current = null)
        {
            return new[] {_lastPerspective}
                .Where(p => p != null)
                .Concat(Perspectives)
                .FirstOrDefault(p => p != current && p.Valid && p.AutoActivate);
        }

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();
            _character?.Dispose();
            _perspective?.Dispose();

            base.Dispose(disposing);
        }
    }
}
