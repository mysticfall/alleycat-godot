using System;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.Control
{
    public class ActionAxisInput : AxisInput
    {
        public string PositiveAction
        {
            get => _positiveAction.TrimToOption().Head();
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _positiveAction = value;
            }
        }

        public string NegativeAction
        {
            get => _negativeAction.TrimToOption().Head();
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _negativeAction = value;
            }
        }

        [Export]
        public bool Polling { get; set; } = true;

        public override bool Valid => base.Valid && 
                                      !string.IsNullOrWhiteSpace(_positiveAction) &&
                                      !string.IsNullOrWhiteSpace(_negativeAction);

        protected float PositiveValue => PositiveAction != null && Input.IsActionPressed(PositiveAction) ? 1f : 0f;

        protected float NegativeValue => NegativeAction != null && Input.IsActionPressed(NegativeAction) ? -1f : 0f;

        protected float Value => PositiveValue + NegativeValue;

        [Export] private string _positiveAction;

        [Export] private string _negativeAction;

        protected override IObservable<float> CreateRawObservable()
        {
            if (Polling)
            {
                return this.OnProcess().Select(_ => Value);
            }

            var input = this.OnInput();

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
