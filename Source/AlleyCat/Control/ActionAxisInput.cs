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
            var positive = ObserveAction(PositiveAction);
            var negative = ObserveAction(NegativeAction);

            return positive.Merge(negative.Select(v => -v)).DistinctUntilChanged(new NonZeroValueComparer());
        }

        private IObservable<float> ObserveAction(string action)
        {
            var pressed = Source.OnInput
                .Where(e => e.IsActionPressed(action))
                .Select(_ => 1f);

            var released = Source.OnInput
                .Where(e => e.IsActionReleased(action))
                .Select(_ => 0f);

            return pressed.Merge(released);
        }
    }

    internal struct NonZeroValueComparer : IEqualityComparer<float>
    {
        private const float Threshold = 0.0000001f;

        public bool Equals(float x, float y) => Mathf.Abs(x) < Threshold && Mathf.Abs(y) < Threshold;

        public int GetHashCode(float obj) => obj.GetHashCode();
    }
}
