using AlleyCat.Action;
using EnsureThat;

namespace AlleyCat.Control
{
    public class PlayerActionDelegate : PlayerAction
    {
        public string Action { get; }

        public PlayerActionDelegate(
            string key,
            string displayName,
            string action,
            ITriggerInput input,
            IPlayerControl playerControl,
            bool active = true) : base(key, displayName, input, playerControl, active)
        {
            Ensure.That(action, nameof(action)).IsNotNull();

            Action = action;
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            context.Actor.Bind(a => a.Actions.Find(Action)).Iter(a => a.Execute(context));
        }

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context.Actor.Bind(a => a.Actions.Find(Action)).Exists(a => a.AllowedFor(context));
        }
    }
}
