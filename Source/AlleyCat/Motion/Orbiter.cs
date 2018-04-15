using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Motion
{
    public abstract class Orbiter : AutowiredNode, IOrbiter
    {
        [Export]
        public bool Active { get; set; } = true;

        public virtual bool Valid => Target != null;

        public abstract Spatial Target { get; }

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

        public float Distance
        {
            get => _distance.Value;
            set => _distance.Value = DistanceRange.Clamp(value);
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

        public IObservable<float> OnDistanceChange => _distance.Where(v => Active && Valid);

        public virtual Range<float> PitchRange => new Range<float>(-Mathf.Pi / 2f, Mathf.Pi / 2f);

        public virtual Range<float> YawRange => new Range<float>(-Mathf.Pi, Mathf.Pi);

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

        private readonly ReactiveProperty<Vector2> _rotation;

        private readonly ReactiveProperty<float> _distance;

        protected Orbiter()
        {
            _rotation = new ReactiveProperty<Vector2>();
            _distance = new ReactiveProperty<float>();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!Active) return;

            Target.GlobalTransform = TargetTransform;
        }

        protected override void Dispose(bool disposing)
        {
            _rotation?.Dispose();
            _distance?.Dispose();

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
