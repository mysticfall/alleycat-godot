using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Logging;
using Godot;
using Microsoft.Extensions.Logging;
using static AlleyCat.Common.MathUtils;
using static LanguageExt.Prelude;

namespace AlleyCat.Motion
{
    public abstract class TurretLike : GameNode, ITurretLike
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public abstract Vector3 Origin { get; }

        public abstract Vector3 Up { get; }

        public abstract Vector3 Forward { get; }

        public Vector3 Right => Forward.Cross(Up);

        public float Pitch
        {
            get => Rotation.y;
            set => Rotation = new Vector2(Yaw, PitchRange.Clamp(value));
        }

        public float Yaw
        {
            get => Rotation.x;
            set => Rotation = new Vector2(YawRange.Clamp(value), Pitch);
        }

        public Vector2 Rotation
        {
            get => _rotation.Value;
            set
            {
                var yaw = YawRange.Clamp(NormalizeAspectAngle(value.x));
                var pitch = PitchRange.Clamp(NormalizeAspectAngle(value.y));

                _rotation.OnNext(new Vector2(yaw, pitch));
            }
        }

        public IObservable<Vector2> OnRotationChange => _rotation.Where(v => Active && Valid);

        public virtual Range<float> YawRange { get; }

        public virtual Range<float> PitchRange { get; }

        private readonly BehaviorSubject<bool> _active;

        private readonly BehaviorSubject<Vector2> _rotation;

        protected TurretLike(
            Range<float> yawRange, 
            Range<float> pitchRange, 
            bool active, 
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            YawRange = yawRange;
            PitchRange = pitchRange;

            _active = CreateSubject(active);
            _rotation = CreateSubject(Vector2.Zero);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                OnRotationChange
                    .DistinctUntilChanged()
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => this.LogTrace("Rotation changed = {}.", v), this);
            }
        }

        public virtual void Reset()
        {
            Rotation = Vector2.Zero;
        }
    }
}
