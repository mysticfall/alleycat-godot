using AlleyCat.Common;
using Godot;
using LanguageExt;

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

        protected override Validation<string, ActionTriggerInput> CreateService()
        {
            return
                from action in Action.TrimToOption()
                    .ToValidation("Action was not specified.")
                select new ActionTriggerInput(
                    GetName(),
                    action,
                    this,
                    Active)
                {
                    UnhandledOnly = UnhandledOnly,
                    StopPropagation = StopPropagation
                };
        }
    }
}
