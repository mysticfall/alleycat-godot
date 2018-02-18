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

        protected float PositiveValue => Input.IsActionPressed(PositiveAction) ? 1f : 0f;

        protected float NegativeValue => Input.IsActionPressed(NegativeAction) ? -1f : 0f;

        protected float Value => PositiveValue + NegativeValue;

        protected override IObservable<float> CreateRawObservable() => 
            this.OnProcess().Select(_ => Value);
    }
}
