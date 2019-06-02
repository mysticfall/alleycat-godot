using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionAxisInput : AxisInput, IActionInput
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

        public IEnumerable<string> Actions { get; }

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

            Actions = Seq(PositiveAction, NegativeAction);
        }

        protected override IObservable<float> CreateRawObservable()
        {
            if (Polling)
            {
                return TimeSource.OnProcess
                    .Select(_ => GetValue())
                    .DistinctUntilChanged(new NonZeroValueComparer());
            }

            var positive = ObserveAction(PositiveAction);
            var negative = ObserveAction(NegativeAction);

            return positive.Merge(negative.Select(v => -v)).DistinctUntilChanged(new NonZeroValueComparer());
        }

        private float GetValue()
        {
            var positive = Input.IsActionPressed(PositiveAction) ? 1f : 0f;
            var negative = Input.IsActionPressed(NegativeAction) ? -1f : 0f;

            return positive + negative;
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

        internal struct NonZeroValueComparer : IEqualityComparer<float>
        {
            private const float Threshold = 0.0000001f;

            public bool Equals(float x, float y) => Mathf.Abs(x) < Threshold && Mathf.Abs(y) < Threshold;

            public int GetHashCode(float obj) => obj.GetHashCode();
        }

        public override bool ConflictsWith(IInput other) =>
            other != this && 
            other is IActionInput input && 
            Actions.Any(input.Actions.Contains);
    }
}
