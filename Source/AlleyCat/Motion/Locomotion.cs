using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    [Singleton(typeof(ILocomotion))]
    public abstract class Locomotion<T> : AutowiredNode, ILocomotion where T : Spatial
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public virtual bool Valid => Target != null;

        [Node(required: false)]
        public T Target { get; set; }

        public Vector3 Velocity => _velocity.Value;

        public Vector3 RotationalVelocity => _rotationalVelocity.Value;

        public IObservable<Vector3> OnVelocityChange => _velocity;

        public IObservable<Vector3> OnRotationalVelocityChange => _rotationalVelocity;

        protected virtual ProcessMode ProcessMode { get; } = ProcessMode.Idle;

        [Export, UsedImplicitly] private NodePath _targetPath;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private readonly ReactiveProperty<Vector3> _velocity = new ReactiveProperty<Vector3>();

        private readonly ReactiveProperty<Vector3> _rotationalVelocity = new ReactiveProperty<Vector3>();

        private Vector3 _requestedMovement;

        private Vector3 _requestedRotation;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _requestedMovement = new Vector3();
            _requestedRotation = new Vector3();

            (ProcessMode == ProcessMode.Idle ? this.OnProcess() : this.OnPhysicsProcess())
                .Where(_ => Active && Valid)
                .Subscribe(HandleProcess)
                .AddTo(this);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Subscribe(_ => this.Stop())
                .AddTo(this);
        }

        public void Move(Vector3 velocity) => _requestedMovement = velocity;

        public void Rotate(Vector3 velocity) => _requestedRotation = velocity;

        protected abstract void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity);

        private void HandleProcess(float delta)
        {
            var before = Target.GlobalTransform;

            Process(delta, _requestedMovement, _requestedRotation);

            var after = Target.GlobalTransform;

            if (delta > 0)
            {
                _velocity.Value = (Target.ToLocal(after.origin) - Target.ToLocal(before.origin)) / delta;
                _rotationalVelocity.Value = (before.basis.Inverse() * after.basis).GetEuler() / delta;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();

            base.Dispose(disposing);
        }
    }
}
