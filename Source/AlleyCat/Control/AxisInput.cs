using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class AxisInput : Input<float>, IAxisInput
    {
        public virtual float Sensitivity
        {
            get => _sensitivity;
            set => _sensitivity = Mathf.Clamp(value, 0, 1);
        }

        public virtual Option<Curve> Curve
        {
            get => Optional(_curve);
            set => _curve = value.ValueUnsafe();
        }

        public virtual float DeadZone
        {
            get => _deadZone;
            set => _deadZone = Mathf.Clamp(value, 0, 1);
        }

        [Export]
        public virtual bool Interpolate { get; set; } = false;

        public virtual float WindowSize
        {
            get => _windowSize;
            set => _windowSize = Mathf.Min(value, 0);
        }

        public virtual float WindowShift
        {
            get => _windowShift;
            set => _windowShift = Mathf.Min(value, 0);
        }

        [Export(PropertyHint.ExpRange, "0, 1, 0.5")]
        private float _sensitivity = 0.5f;

        [Export] private Curve _curve;

        [Export(PropertyHint.ExpRange, "0, 1")]
        private float _deadZone;

        [Export(PropertyHint.ExpRange, "0, 1000, 5")]
        private float _windowSize = 5f;

        [Export(PropertyHint.ExpRange, "0, 1000, 1")]
        private float _windowShift = 1f;

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
                        this.GetIdleScheduler())
                    .Where(v => v.Any())
                    .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count);
            }

            return input;
        }

        protected abstract IObservable<float> CreateRawObservable();
    }
}
