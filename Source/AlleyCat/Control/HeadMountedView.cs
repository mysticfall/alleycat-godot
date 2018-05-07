using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    [Singleton(typeof(IPerspectiveView), typeof(IFirstPersonView))]
    public class HeadMountedView : TurretLike, IFirstPersonView
    {
        public override bool Valid => base.Valid && Character != null && Camera != null && Camera.IsCurrent();

        [Node(required: false)]
        public virtual IHumanoid Character { get; set; }

        [Node(required: false)]
        public virtual Camera Camera { get; set; }

        public bool AutoActivate => true;

        public IVision Vision => Character?.Vision;

        public Vector3 Viewpoint => Vision?.Viewpoint ?? Vector3.Zero;

        public Vector3 LookDirection => Vision?.LookDirection ?? Forward;

        public override Vector3 Origin => Vision?.Origin ?? Vector3.Zero;

        public override Vector3 Up => Vision?.Up ?? Vector3.Up;

        public override Vector3 Forward => Vision?.Forward ?? Vector3.Forward;

        protected virtual IObservable<Vector2> ViewInput => _viewInput.AsVector2Input().Where(_ => Active && Valid);

        [CanBeNull]
        protected virtual IObservable<bool> DeactivateInput =>
            _deactivateInput.GetTrigger().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _characterPath;

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Rotation")] private InputBindings _viewInput;

        [Node("Deactivate", false)] private InputBindings _deactivateInput;

        protected virtual Vector3 FocalPoint =>
            Character?.GlobalTransform().Xform(this.GetBasis().Forward() * 100) ?? Origin;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            ViewInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Do(_ => this.Reset())
                .Do(_ => Vision?.Reset())
                .Subscribe()
                .AddTo(this);

            var onProcess = this.OnProcess().Where(_ => Active && Valid);

            onProcess
                .Where(_ => Active && Valid)
                .Select(_ => FocalPoint)
                .Subscribe(v => Vision?.LookAt(v))
                .AddTo(this);

            onProcess
                .Select(_ => new Transform(Basis.Identity, Viewpoint))
                .Select(t => t.LookingAt(Viewpoint + LookDirection, Up))
                .Subscribe(v => Camera?.SetGlobalTransform(v))
                .AddTo(this);

            DeactivateInput?
                .Subscribe(_ => this.Deactivate())
                .AddTo(this);
        }
    }
}
