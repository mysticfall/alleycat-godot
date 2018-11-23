using AlleyCat.Action;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class PlayerActionDelegate : PlayerAction
    {
        public string Action { get; }

        public PlayerActionDelegate(
            string key,
            string displayName,
            string action,
            Option<IPlayerControl> playerControl,
            ITriggerInput input,
            bool active,
            ILogger logger) : base(key, displayName, playerControl, input, active, logger)
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
