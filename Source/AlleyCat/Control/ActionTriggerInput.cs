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

        protected override IObservable<bool> CreateObservable() => this.OnUnhandledInput()
            .Where(_ => Action != null)
            .Select(e => e.IsActionPressed(Action))
            .Where(v => v);
    }
}
