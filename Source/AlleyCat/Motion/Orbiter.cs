using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Motion
{
    public abstract class Orbiter : Rotatable, IOrbiter
    {
        public override bool Valid => base.Valid && Target != null;

        public abstract Spatial Target { get; }

        public float Distance
        {
            get => _distance.Value;
            set => _distance.Value = DistanceRange.Clamp(value);
        }

        public IObservable<float> OnDistanceChange => _distance.Where(v => Active && Valid);

        public virtual Range<float> DistanceRange => new Range<float>(0.1f, 10f);

        protected virtual Transform TargetTransform
        {
            get
            {
                var pivot = Origin;

                var direction = -Forward
                    .Rotated(Up, Yaw)
                    .Rotated(Right.Rotated(Up, Yaw), Pitch);

                return new Transform(Basis.Identity, pivot)
                    .Translated(direction * Distance)
                    .LookingAt(pivot, Up);
            }
        }

        private readonly ReactiveProperty<float> _distance = new ReactiveProperty<float>();

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            this.OnProcess()
                .Where(_ => Active && Valid)
                .Subscribe(_ => Target.GlobalTransform = TargetTransform)
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _distance?.Dispose();

            base.Dispose(disposing);
        }
    }
}
