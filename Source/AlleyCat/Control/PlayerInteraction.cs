using System.Linq;
using AlleyCat.Action;
using AlleyCat.Character;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Control
{
    public class PlayerInteraction : PlayerAction
    {
        public PlayerInteraction(
            string key,
            string displayName,
            ITriggerInput input,
            IPlayerControl playerControl,
            bool active = true) : base(key, displayName, input, playerControl, active)
        {
        }

        protected override Option<IActionContext> CreateActionContext(IHumanoid player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return PlayerControl.FocusedObject
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

            return Player.SequenceEqual(context.Actor) && PlayerControl.FocusedObject.IsSome;
        }
    }
}
