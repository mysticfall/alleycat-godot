using System;
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
    [Singleton(typeof(IPerspectiveView), typeof(IFirstPersonView))]
    public class HeadMountedView : Rotatable, IFirstPersonView
    {
        public override bool Valid => base.Valid && Character != null && Camera != null && Camera.IsCurrent();

        [Node(required: false)]
        public virtual IHumanoid Character { get; set; }

        [Node(required: false)]
        public virtual Camera Camera { get; set; }

        public bool AutoActivate => true;

        public override Vector3 Origin => Character?.Vision.Origin ?? Vector3.Zero;

        public override Vector3 Up => Character?.Vision.Up ?? Vector3.Up;

        public override Vector3 Forward => Character?.Vision.Forward ?? Vector3.Forward;

        protected virtual IObservable<Vector2> ViewInput => _viewInput.AsVector2Input().Where(_ => Active && Valid);

        [CanBeNull]
        protected virtual IObservable<bool> DeactivateInput =>
            _deactivateInput.GetTrigger().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _character;

        [Export, UsedImplicitly] private NodePath _camera;

        [Node("Rotation")] private InputBindings _viewInput;

        [Node("Deactivate", false)] private InputBindings _deactivateInput;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            ViewInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            OnRotationChange.Merge(
                    OnActiveStateChange
                        .Where(_ => Valid)
                        .Select(v => v ? Rotation : Vector2.Zero))
                // ReSharper disable once PossibleNullReferenceException
                .Subscribe(v => Character.Vision.Rotation = v)
                .AddTo(this);

            this.OnProcess()
                .Where(_ => Active && Valid)
                .Select(_ => new Transform(Basis.Identity, Origin).LookingAt(Origin + Forward, Up))
                // ReSharper disable once PossibleNullReferenceException
                .Subscribe(_ => Camera.GlobalTransform = _)
                .AddTo(this);

            DeactivateInput?
                .Subscribe(_ => this.Deactivate())
                .AddTo(this);
        }
    }
}
