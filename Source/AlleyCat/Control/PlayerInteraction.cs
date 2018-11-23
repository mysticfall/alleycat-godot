using System.Linq;
using AlleyCat.Action;
using AlleyCat.Character;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class PlayerInteraction : PlayerAction
    {
        public PlayerInteraction(
            string key,
            string displayName,
            Option<IPlayerControl> playerControl,
            ITriggerInput input,
            bool active,
            ILogger logger) : base(key, displayName, playerControl, input, active, logger)
        {
        }

        protected override Option<IActionContext> CreateActionContext(IHumanoid player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return PlayerControl
                .Bind(c => c.FocusedObject)
                .Map<IEntity, IActionContext>(entity => new InteractionContext(player, entity))
                .HeadOrNone();
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            Player
                .Bind(p => p.Actions.Values)
                .OfType<Interaction>()
                .Find(a => a.AllowedFor(context))
                .Iter(p => p.Execute(context));
        }

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return Player.SequenceEqual(context.Actor) && PlayerControl.Bind(c => c.FocusedObject).IsSome;
        }
    }
}
