using System;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionTriggerInput : Input<bool>, ITriggerInput
    {
        public string Action
        {
            get => _action.TrimToOption().Head();
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _action = value;
            }
        }

        public override bool Valid => base.Valid && !string.IsNullOrWhiteSpace(_action);

        [Export]
        public bool UnhandledOnly { get; set; } = true;

        [Export]
        public bool StopPropagation { get; set; } = true;

        [Export] private string _action;

        protected override IObservable<bool> CreateObservable()
        {
            var input = UnhandledOnly ? this.OnUnhandledInput() : this.OnInput();

            return input
                .Select(e => e.IsActionPressed(Action))
                .Where(identity)
                .Do(_ =>
                {
                    if (Active && StopPropagation) GetTree().SetInputAsHandled();
                });
        }
    }
}
