using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public abstract class Orbiter : TurretLike, IOrbiter
    {
        public abstract Spatial Target { get; }

        public float Distance
        {
            get => _distance.Value;
            set => _distance.OnNext(DistanceRange.Clamp(value));
        }

        public virtual float InitialDistance
        {
            get => _initialDistance;
            set => _initialDistance = DistanceRange.Clamp(value);
        }

        public IObservable<float> OnDistanceChange => _distance.Where(v => Active && Valid);

        public virtual Range<float> DistanceRange => new Range<float>(_minDistance, _maxDistance);

        public Vector3 Offset
        {
            get => _offset.Value;
            set => _offset.OnNext(value);
        }

        // ReSharper disable once ConvertToAutoProperty
        public virtual Vector3 InitialOffset
        {
            get => _initialOffset;
            set => _initialOffset = value;
        }

        public IObservable<Vector3> OnOffsetChange => _offset.Where(v => Active && Valid);

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

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

        [Export, UsedImplicitly] private float _minDistance;

        [Export, UsedImplicitly] private float _maxDistance;

        [Export] private float _initialDistance = 0.8f;

        [Export] private Vector3 _initialOffset = Vector3.Zero;

        private readonly BehaviorSubject<float> _distance;

        private readonly BehaviorSubject<Vector3> _offset;

        protected Orbiter() : this(
            new Range<float>(-180f, 180f), 
            new Range<float>(-90f, 90f), 
            new Range<float>(0.1f, 10f))
        {
        }

        protected Orbiter(
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange) : base(yawRange, pitchRange)
        {
            _minDistance = Mathf.Max(0, distanceRange.Min);
            _maxDistance = distanceRange.Max;

            _distance = new BehaviorSubject<float>(_initialDistance).AddTo(this);
            _offset = new BehaviorSubject<Vector3>(_initialOffset).AddTo(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.OnProcess(ProcessMode)
                .Where(_ => Active && Valid)
                .Subscribe(_ => Target.GlobalTransform = TargetTransform)
                .AddTo(this);
        }

        public override void Reset()
        {
            base.Reset();

            Distance = InitialDistance;
            Offset = InitialOffset;
        }
    }
}
