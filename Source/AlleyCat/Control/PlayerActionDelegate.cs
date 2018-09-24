using AlleyCat.Action;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public class PlayerActionDelegate : PlayerAction
    {
        [Export, UsedImplicitly]
        public string Action { get; private set; }

        public override bool Valid => base.Valid && !string.IsNullOrEmpty(Action);

        protected override void DoExecute(IActionContext context)
        {
            IAction action = null;

            context?.Actor?.Actions.TryGetValue(Action, out action);

            action?.Execute(context);
        }

        public override bool AllowedFor(IActionContext context)
        {
            IAction action = null;

            context?.Actor?.Actions.TryGetValue(Action, out action);

            return action?.AllowedFor(context) ?? false;
        }
    }
}
