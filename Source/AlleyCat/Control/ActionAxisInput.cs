using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Control
{
    public class ActionAxisInput : AxisInput
    {
        [Export]
        public string PositiveAction { get; set; }

        [Export]
        public string NegativeAction { get; set; }

        [Export]
        public bool Polling { get; set; } = true;

        protected float PositiveValue => PositiveAction != null && Input.IsActionPressed(PositiveAction) ? 1f : 0f;

        protected float NegativeValue => NegativeAction != null && Input.IsActionPressed(NegativeAction) ? -1f : 0f;

        protected float Value => PositiveValue + NegativeValue;

        protected override IObservable<float> CreateRawObservable()
        {
            if (Polling)
            {
                return this.OnProcess().Select(_ => Value);
            }

            var input = this.OnUnhandledInput();

            var positive = input
                .Where(_ => PositiveAction != null)
                .Where(e => e.IsActionPressed(PositiveAction))
                .Select(_ => 1f);

            var negative = input
                .Where(_ => NegativeAction != null)
                .Where(e => e.IsActionPressed(NegativeAction))
                .Select(_ => -1f);

            return positive.Merge(negative);
        }
    }
}
