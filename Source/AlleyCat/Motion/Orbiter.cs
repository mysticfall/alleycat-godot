using System;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public abstract class Orbiter : TurretLike, IOrbiter
    {
        public override bool Valid => base.Valid && Target != null;

        public abstract Spatial Target { get; }

        public float Distance
        {
            get => _distance.Value;
            set => _distance.Value = DistanceRange.Clamp(value);
        }

        // ReSharper disable once ConvertToAutoProperty
        public virtual float InitialDistance
        {
            get => _initialDistance;
            set => _initialDistance = DistanceRange.Clamp(value);
        }

        public IObservable<float> OnDistanceChange => _distance.Where(v => Active && Valid);

        public virtual Range<float> DistanceRange => new Range<float>(0.1f, 10f);

        public Vector3 Offset
        {
            get => _offset.Value;
            set => _offset.Value = value;
        }

        // ReSharper disable once ConvertToAutoProperty
        public virtual Vector3 InitialOffset
        {
            get => _initialOffset;
            set => _initialOffset = value;
        }

        public IObservable<Vector3> OnOffsetChange => _offset.Where(v => Active && Valid);

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
                    .LookingAt(pivot, Up)
                    .Translated(Offset);
            }
        }

        [Export, UsedImplicitly] private float _initialDistance = 0.8f;

        [Export, UsedImplicitly] private Vector3 _initialOffset = Vector3.Zero;

        private readonly ReactiveProperty<float> _distance = new ReactiveProperty<float>();

        private readonly ReactiveProperty<Vector3> _offset = new ReactiveProperty<Vector3>();

        protected Orbiter()
        {
            ProcessMode = ProcessMode.Idle;
        }

        protected Orbiter(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
            ProcessMode = ProcessMode.Idle;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            OnLoop
                .Where(_ => Active && Valid)
                .Subscribe(_ => Target.GlobalTransform = TargetTransform)
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _distance?.Dispose();
            _offset?.Dispose();

            base.Dispose(disposing);
        }

        public override void Reset()
        {
            base.Reset();

            Distance = InitialDistance;
            Offset = InitialOffset;
        }
    }
}
