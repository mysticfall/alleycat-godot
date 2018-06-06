using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    [Singleton(typeof(IPerspectiveView))]
    public class FreeCameraView : TurretLike, IPerspectiveView
    {
        public override bool Valid => base.Valid && Character != null && Camera != null && Camera.IsCurrent();

        public virtual IHumanoid Character { get; set; }

        [Node(required: false)]
        public virtual Camera Camera { get; private set;  }

        public bool AutoActivate => false;

        public override Vector3 Origin => Camera?.GlobalTransform.origin ?? Vector3.Zero;

        public override Vector3 Forward => Camera?.GlobalTransform.Forward() ?? Vector3.Forward;

        public override Vector3 Up => Vector3.Up;

        protected IObservable<Vector2> RotationInput => _rotationInput.AsVector2Input().Where(_ => Active && Valid);

        protected IObservable<Vector2> MovementInput => _movementInput.AsVector2Input().Where(_ => Active && Valid);

        [CanBeNull]
        protected virtual IObservable<bool> ToggleInput => _toggleInput.GetTrigger().Where(_ => Valid);

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Rotation")] private InputBindings _rotationInput;

        [Node("Movement")] private InputBindings _movementInput;

        [Node("Toggle", false)] private InputBindings _toggleInput;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Camera = Camera ?? GetViewport().GetCamera();

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

            OnActiveStateChange
                .Where(_ => Valid)
                // ReSharper disable once PossibleNullReferenceException
                .Subscribe(v => Character.Locomotion.Active = !v)
                .AddTo(this);
        }
    }
}
