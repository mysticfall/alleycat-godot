using AlleyCat.Action;
using AlleyCat.Character;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class PlayerAction : InputAction
    {
        public override bool Valid => base.Valid && Player.IsSome;

        public Option<IHumanoid> Player => PlayerControl.Bind(c => c.Character);

        public Option<IPlayerControl> PlayerControl { get; }

        protected PlayerAction(
            string key,
            string displayName,
            Option<IPlayerControl> playerControl,
            ITriggerInput input,
            bool active = true) : base(key, displayName, input, active)
        {
            PlayerControl = playerControl;
        }

        protected override Option<IActionContext> CreateActionContext() => Player.Bind(CreateActionContext);

        protected virtual Option<IActionContext> CreateActionContext(IHumanoid player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return new ActionContext(Some<IActor>(player));
        }
    }
}
