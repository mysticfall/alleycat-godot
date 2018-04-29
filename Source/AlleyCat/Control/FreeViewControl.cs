using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public class FreeViewControl : AutowiredNode, IActivatable, IValidatable
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public virtual bool Valid => Target != null;

        [Node]
        public Spatial Target { get; private set; }

        protected IObservable<Vector2> RotationInput => _rotationInput.AsVector2Input().Where(_ => Active && Valid);

        protected IObservable<Vector2> MovementInput => _movementInput.AsVector2Input().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _target = "..";

        [Node("Rotation")] private InputBindings _rotationInput;

        [Node("Movement")] private InputBindings _movementInput;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        [PostConstruct]
        private void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            RotationInput
                .Subscribe(v =>
                {
                    Target.GlobalRotate(new Vector3(0, 1, 0), -v.x);
                    Target.RotateObjectLocal(new Vector3(1, 0, 0), -v.y);
                })
                .AddTo(this);

            MovementInput
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Select(v => v * 0.1f)
                .Subscribe(Target.TranslateObjectLocal)
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();

            base.Dispose(disposing);
        }
    }
}
