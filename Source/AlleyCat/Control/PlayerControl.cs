using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    [AutowireContext, Singleton(typeof(IPlayerControl))]
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
        public IHumanoid Character { get; set; }

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

        protected IObservable<Vector2> MovementInput => _movementInput.AsVector2Input().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Movement")] private InputBindings _movementInput;

        private readonly ReactiveProperty<IPerspectiveView> _perspective = new ReactiveProperty<IPerspectiveView>();

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private IPerspectiveView _lastPerspective;

        [PostConstruct]
        protected void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Camera = Camera ?? GetViewport().GetCamera();
            Character = Character ?? GetTree().GetNodesInGroup<IHumanoid>(Tags.Player).FirstOrDefault();

            IPerspectiveView active = null;

            foreach (var perspective in Perspectives)
            {
                perspective.Camera = Camera;
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
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Subscribe(v => Character?.Locomotion.Move(v))
                .AddTo(this);

            OnPerspectiveChange
                .Pairwise()
                .Subscribe(t => OnPerspectiveChanged(t.Item1, t.Item2))
                .AddTo(this);
        }

        protected virtual void OnPerspectiveChanged(IPerspectiveView previous, IPerspectiveView current)
        {
            if (previous is ITurretLike previousRotatable && current is ITurretLike currentRotatable)
            {
                currentRotatable.Rotation = previousRotatable.Rotation;
            }

            _lastPerspective = previous != null && previous.AutoActivate ? previous : null;

            current?.Activate();
            previous?.Deactivate();
        }

        protected IPerspectiveView FindNextValidPerspective([CanBeNull] IPerspectiveView current = null) =>
            new[] {_lastPerspective}
                .Where(p => p != null)
                .Concat(Perspectives)
                .FirstOrDefault(p => p != current && p.Valid && p.AutoActivate);

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!Active || !Valid) return;

            // ReSharper disable once PossibleNullReferenceException
            if (!Character.Locomotion.IsMoving())
            {
                Character.Locomotion.Rotate(Vector3.Zero);
            }
            else if (Perspective is ITurretLike rotatable)
            {
                var offset = Mathf.Lerp(0, rotatable.Yaw, delta * 1.5f);

                Character.Locomotion.Rotate(Vector3.Up * offset);
                rotatable.Yaw -= offset;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();
            _perspective?.Dispose();

            base.Dispose(disposing);
        }
    }
}
