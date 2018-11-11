using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;

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

        public virtual Range<float> DistanceRange { get; }

        public float InitialDistance
        {
            get => _initialDistance;
            set => _initialDistance = DistanceRange.Clamp(value);
        }

        public virtual IObservable<float> OnDistanceChange => _distance.Where(v => Active && Valid);

        public Vector3 Offset { get; set; }

        public Vector3 InitialOffset { get; set; }

        public virtual IObservable<Vector3> OnOffsetChange => _offset.Where(v => Active && Valid);

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

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

        private readonly BehaviorSubject<float> _distance;

        private readonly BehaviorSubject<Vector3> _offset;

        private float _initialDistance;

        protected Orbiter(
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange,
            float initialDistance,
            Vector3 initialOffset,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active = true) : base(yawRange, pitchRange, active)
        {
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            DistanceRange = distanceRange;

            InitialDistance = initialDistance;
            InitialOffset = initialOffset;

            ProcessMode = processMode;
            TimeSource = timeSource;

            _distance = new BehaviorSubject<float>(InitialDistance).AddTo(this);
            _offset = new BehaviorSubject<Vector3>(InitialOffset).AddTo(this);
        }

        protected override void PostConstruct()
        {
            TimeSource.OnProcess(ProcessMode)
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
