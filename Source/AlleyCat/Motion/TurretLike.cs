using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Motion
{
    public abstract class TurretLike : AutowiredNode, ITurretLike
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public virtual bool Valid => true;

        public abstract Vector3 Origin { get; }

        public abstract Vector3 Up { get; }

        public abstract Vector3 Forward { get; }

        public Vector3 Right => Forward.Cross(Up);

        public float Pitch
        {
            get => Rotation.y;
            set => Rotation = new Vector2(Yaw, value);
        }

        public float Yaw
        {
            get => Rotation.x;
            set => Rotation = new Vector2(value, Pitch);
        }

        public Vector2 Rotation
        {
            get => _rotation.Value;
            set
            {
                var yaw = YawRange.Clamp(NormalizeAspectAngle(value.x));
                var pitch = PitchRange.Clamp(NormalizeAspectAngle(value.y));

                _rotation.Value = new Vector2(yaw, pitch);
            }
        }

        public IObservable<Vector2> OnRotationChange => _rotation.Where(v => Active && Valid);

        public virtual Range<float> YawRange => new Range<float>(Mathf.Deg2Rad(_minYaw), Mathf.Deg2Rad(_maxYaw));

        public virtual Range<float> PitchRange => new Range<float>(Mathf.Deg2Rad(_minPitch), Mathf.Deg2Rad(_maxPitch));

        [Export, UsedImplicitly] private float _maxYaw;

        [Export, UsedImplicitly] private float _minYaw;

        [Export, UsedImplicitly] private float _maxPitch;

        [Export, UsedImplicitly] private float _minPitch;

        private readonly ReactiveProperty<Vector2> _rotation = new ReactiveProperty<Vector2>();

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        protected TurretLike() : this(new Range<float>(-180f, 180f), new Range<float>(-90f, 90f))
        {
        }

        protected TurretLike(Range<float> yawRange, Range<float> pitchRange)
        {
            _minYaw = yawRange.Min;
            _maxYaw = yawRange.Max;

            _minPitch = pitchRange.Min;
            _maxPitch = pitchRange.Max;
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Reset();
        }

        public virtual void Reset()
        {
            Rotation = Vector2.Zero;
        }

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();
            _rotation?.Dispose();

            base.Dispose(disposing);
        }

        private static float NormalizeAspectAngle(float angle)
        {
            var value = angle;

            while (value < 0) value += 2 * Mathf.Pi;

            return value > Mathf.Pi ? value - 2 * Mathf.Pi : value;
        }
    }
}
