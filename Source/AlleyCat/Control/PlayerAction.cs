using System;
using AlleyCat.Action;
using AlleyCat.Character;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class PlayerAction : InputAction
    {
        public override bool Valid => base.Valid && Player.IsSome;

        protected Option<IHumanoid> Player => PlayerControl.Character;

        protected IPlayerControl PlayerControl { get; }

        protected PlayerAction(
            string key,
            string displayName,
            ITriggerInput input,
            IPlayerControl playerControl,
            bool active = true) : base(key, displayName, input, active)
        {
            Ensure.That(playerControl, nameof(playerControl)).IsNotNull();

            PlayerControl = playerControl;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            PlayerControl.OnActiveStateChange
                .Subscribe(v => Active = v)
                .AddTo(this);
        }

        protected override Option<IActionContext> CreateActionContext() => Player.Bind(CreateActionContext);

        protected virtual Option<IActionContext> CreateActionContext(IHumanoid player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return new ActionContext(Some((IActor) player));
        }
    }
}
