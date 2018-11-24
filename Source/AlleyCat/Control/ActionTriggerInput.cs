using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionTriggerInput : Input<bool>, ITriggerInput
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

        private string _action;

        public ActionTriggerInput(
            string key,
            string action,
            IInputSource source,
            bool active,
            ILoggerFactory loggerFactory) : base(key, source, active, loggerFactory)
        {
            Action = action;
        }

        protected override IObservable<bool> CreateObservable()
        {
            var input = UnhandledOnly ? Source.OnUnhandledInput : Source.OnInput;

            return input
                .Select(e => e.IsActionPressed(Action))
                .Where(identity)
                .Do(_ =>
                {
                    if (Active && StopPropagation) Source.SetInputAsHandled();
                });
        }
    }
}
