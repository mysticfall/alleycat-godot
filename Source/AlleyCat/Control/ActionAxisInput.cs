using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public class ActionAxisInput : AxisInput
    {
        [Export, NotNull]
        public string PositiveAction { get; set; } = "move_right";

        [Export, NotNull]
        public string NegativeAction { get; set; } = "move_left";

        [Export]
        public bool Polling { get; set; } = true;

        protected float PositiveValue => Input.IsActionPressed(PositiveAction) ? 1f : 0f;

        protected float NegativeValue => Input.IsActionPressed(NegativeAction) ? -1f : 0f;

        protected float Value => PositiveValue + NegativeValue;

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
