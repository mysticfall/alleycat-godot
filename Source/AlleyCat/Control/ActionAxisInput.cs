using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;

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
            bool active = true) : base(key, source, timeSource, active)
        {
            PositiveAction = positiveAction;
            NegativeAction = negativeAction;
        }

        protected override IObservable<float> CreateRawObservable()
        {
            if (Polling)
            {
                return TimeSource.OnProcess.Select(_ => Value);
            }

            var input = Source.OnInput;

            var positive = input
                .Where(e => e.IsActionPressed(PositiveAction))
                .Select(_ => 1f);

            var negative = input
                .Where(e => e.IsActionPressed(NegativeAction))
                .Select(_ => -1f);

            return positive.Merge(negative);
        }
    }
}
