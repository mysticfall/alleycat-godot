using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionTriggerInput : Input<bool>, ITriggerInput, IActionInput
    {
        public string Action
        {
            get => _action;
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _action = value;
            }
        }

        public bool UnhandledOnly { get; set; } = true;

        public bool StopPropagation { get; set; } = true;

        public IEnumerable<string> Actions { get; }

        private string _action;

        public ActionTriggerInput(
            string key,
            string action,
            IInputSource source,
            bool active,
            ILoggerFactory loggerFactory) : base(key, source, active, loggerFactory)
        {
            Action = action;
            Actions = Seq1(action);
        }

        protected override IObservable<bool> CreateObservable()
        {
            var input = UnhandledOnly ? Source.OnUnhandledInput : Source.OnInput;

            return input
                .Select(e => (pressed: e.IsActionPressed(Action), released: e.IsActionReleased(Action)))
                .Where(v => v.pressed || v.released)
                .Select(v => v.pressed)
                .DistinctUntilChanged()
                .Do(_ =>
                {
                    if (Active && StopPropagation) Source.SetInputAsHandled();
                });
        }

        public override bool ConflictsWith(IInput other) =>
            other != this &&
            other is IActionInput input &&
            Actions.Any(input.Actions.Contains);
    }
}
