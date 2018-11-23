using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public abstract class AxisInput : Input<float>, IAxisInput
    {
        public float Sensitivity
        {
            get => _sensitivity;
            set => _sensitivity = Mathf.Clamp(value, 0, 1);
        }

        public Option<Curve> Curve { get; set; }

        public virtual float DeadZone
        {
            get => _deadZone;
            set => _deadZone = Mathf.Clamp(value, 0, 1);
        }

        public bool Interpolate { get; set; }

        public float WindowSize
        {
            get => _windowSize;
            set => _windowSize = Mathf.Max(value, 0);
        }

        public float WindowShift
        {
            get => _windowShift;
            set => _windowShift = Mathf.Max(value, 0);
        }

        protected ITimeSource TimeSource { get; }

        private float _sensitivity = 0.5f;

        private float _deadZone;

        private float _windowSize = 5f;

        private float _windowShift = 1f;

        protected AxisInput(
            string key,
            IInputSource source,
            ITimeSource timeSource,
            bool active,
            ILogger logger) : base(key, source, active, logger)
        {
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            TimeSource = timeSource;
        }

        protected override IObservable<float> CreateObservable()
        {
            var input = CreateRawObservable()
                .Where(v => Math.Abs(v) >= DeadZone)
                .Select(v => v * Sensitivity)
                .Select(v => Curve.Map(c => c.Interpolate(v)).IfNone(v));

            if (Interpolate)
            {
                input = input
                    .Buffer(
                        TimeSpan.FromMilliseconds(WindowSize),
                        TimeSpan.FromMilliseconds(WindowShift),
                        TimeSource.Scheduler)
                    .Where(v => v.Any())
                    .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count);
            }

            return input;
        }

        protected abstract IObservable<float> CreateRawObservable();
    }
}
