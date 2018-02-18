using System;
using System.Linq;
using System.Reactive.Linq;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public abstract class AxisInput : Input<float>, IAxisInput
    {
        [Export(PropertyHint.ExpRange, "0, 1")]
        public virtual float Sensitivity { get; set; } = 0.5f;

        [Export]
        public virtual Curve Curve { get; set; }

        [Export(PropertyHint.ExpRange, "0, 1")]
        public virtual float DeadZone { get; set; } = 0f;

        [Export]
        public virtual bool Interpolate { get; set; } = false;

        [Export(PropertyHint.ExpRange, "0, 1000, 1")]
        public virtual float WindowSize { get; set; } = 100f;

        [Export(PropertyHint.ExpRange, "0, 1000, 1")]
        public virtual float WindowShift { get; set; } = 5f;

        protected override IObservable<float> CreateObservable()
        {
            var input = CreateRawObservable()
                .Where(v => Math.Abs(v) >= DeadZone)
                .Select(v => v * Sensitivity)
                .Select(v => Curve?.Interpolate(v) ?? v);

            if (Interpolate)
            {
                input = input
                    .Buffer(
                        TimeSpan.FromMilliseconds(WindowSize),
                        TimeSpan.FromMilliseconds(WindowShift))
                    .Where(v => v.Any())
                    .Select(v => v.Aggregate((v1, v2) => v1 + v2) / v.Count);
            }

            return input;
        }

        [NotNull]
        protected abstract IObservable<float> CreateRawObservable();
    }
}
