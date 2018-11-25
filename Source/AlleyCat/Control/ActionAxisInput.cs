using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class ActionAxisInput : AxisInput
    {
        public string PositiveAction
        {
            get => _positiveAction;
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _positiveAction = value;
            }
        }

        public string NegativeAction
        {
            get => _negativeAction;
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _negativeAction = value;
            }
        }

        public bool Polling { get; set; } = true;

        protected float PositiveValue => Input.IsActionPressed(PositiveAction) ? 1f : 0f;

        protected float NegativeValue => Input.IsActionPressed(NegativeAction) ? -1f : 0f;

        protected float Value => PositiveValue + NegativeValue;

        private string _positiveAction;

        private string _negativeAction;

        public ActionAxisInput(
            string key,
            string positiveAction,
            string negativeAction,
            IInputSource source,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(key, source, timeSource, active, loggerFactory)
        {
            PositiveAction = positiveAction;
            NegativeAction = negativeAction;
        }

        protected override IObservable<float> CreateRawObservable()
        {
            if (Polling)
            {
                return TimeSource.OnProcess
                    .Select(_ => Value)
                    .DistinctUntilChanged(new NonZeroValueComparer());
            }

            var input = Source.OnInput;

            var positive = input
                .Where(e => e.IsActionPressed(PositiveAction))
                .Select(_ => 1f);

            var negative = input
                .Where(e => e.IsActionPressed(NegativeAction))
                .Select(_ => -1f);

            return positive.Merge(negative).DistinctUntilChanged(new NonZeroValueComparer());
        }
    }

    internal struct NonZeroValueComparer : IEqualityComparer<float>
    {
        private const float Threshold = 0.0000001f;

        public bool Equals(float x, float y) => Mathf.Abs(x) < Threshold && Mathf.Abs(y) < Threshold;

        public int GetHashCode(float obj) => obj.GetHashCode();
    }
}
