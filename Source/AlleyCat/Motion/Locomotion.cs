using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

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

        public override bool Valid => base.Valid && _target.IsSome;

        public T Target => _target.Head();

        public Vector3 Velocity => _velocity.Value;

        public Vector3 RotationalVelocity => _rotationalVelocity.Value;

        public IObservable<Vector3> OnVelocityChange => _velocity;

        public IObservable<Vector3> OnRotationalVelocityChange => _rotationalVelocity;

        [Export] private NodePath _targetPath;

        [Node] private Option<T> _target = None;

        private readonly ReactiveProperty<bool> _active;

        private readonly ReactiveProperty<Vector3> _velocity;

        private readonly ReactiveProperty<Vector3> _rotationalVelocity;

        private Vector3 _requestedMovement;

        private Vector3 _requestedRotation;

        protected Locomotion()
        {
            _active = new ReactiveProperty<bool>(true).AddTo(this);
            _velocity = new ReactiveProperty<Vector3>().AddTo(this);
            _rotationalVelocity = new ReactiveProperty<Vector3>().AddTo(this);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _requestedMovement = new Vector3();
            _requestedRotation = new Vector3();

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Subscribe(_ => this.Stop())
                .AddTo(this);
        }

        public void Move(Vector3 velocity) => _requestedMovement = velocity;

        public void Rotate(Vector3 velocity) => _requestedRotation = velocity;

        protected abstract void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity);

        protected override void ProcessLoop(float delta)
        {
            base.ProcessLoop(delta);

            if (!Active || !Valid) return;

            var before = Target.GlobalTransform;

            Process(delta, _requestedMovement, _requestedRotation);

            var after = Target.GlobalTransform;

            if (delta <= 0) return;

            _velocity.Value = (Target.ToLocal(after.origin) - Target.ToLocal(before.origin)) / delta;
            _rotationalVelocity.Value = (before.basis.Inverse() * after.basis).GetEuler() / delta;
        }
    }
}
