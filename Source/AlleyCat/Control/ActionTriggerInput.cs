using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Control
{
    public class ActionTriggerInput : Input<bool>, ITriggerInput
    {
        [Export]
        public string Action { get; set; }

        [Export]
        public bool UnhandledOnly { get; set; } = true;

        [Export]
        public bool StopPropagation { get; set; } = true;

        protected override IObservable<bool> CreateObservable()
        {
            var input = UnhandledOnly ? this.OnUnhandledInput() : this.OnInput();

            return input
                .Where(_ => Action != null)
                .Select(e => e.IsActionPressed(Action))
                .Where(v => v)
                .Do(_ =>
                {
                    if (Active && StopPropagation) GetTree().SetInputAsHandled();
                });
        }
    }
}
