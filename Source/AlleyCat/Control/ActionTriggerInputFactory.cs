using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class ActionTriggerInputFactory : InputFactory<ActionTriggerInput, bool>
    {
        [Export]
        public string Action { get; set; }

        [Export]
        public bool UnhandledOnly { get; set; } = true;

        [Export]
        public bool StopPropagation { get; set; } = true;

        protected override Validation<string, ActionTriggerInput> CreateService(ILogger logger)
        {
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return
                from action in Action.TrimToOption()
                    .ToValidation("Action was not specified.")
                select new ActionTriggerInput(
                    GetName(),
                    action,
                    this,
                    Active,
                    logger)
                {
                    UnhandledOnly = UnhandledOnly,
                    StopPropagation = StopPropagation
                };
        }
    }
}
