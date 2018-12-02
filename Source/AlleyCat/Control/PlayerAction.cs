using System;
using AlleyCat.Action;
using AlleyCat.Character;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class PlayerAction : InputAction
    {
        public override bool Valid => base.Valid && Player.IsSome;

        public Option<IHumanoid> Player => PlayerControl.Bind(c => c.Character);

        public Option<IPlayerControl> PlayerControl => _playerControl.Invoke();

        private readonly Func<Option<IPlayerControl>> _playerControl;

        protected PlayerAction(
            string key,
            string displayName,
            Func<Option<IPlayerControl>> playerControl,
            ITriggerInput input,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, input, active, loggerFactory)
        {
            Ensure.That(playerControl, nameof(playerControl)).IsNotNull();

            _playerControl = playerControl;
        }

        protected override Option<IActionContext> CreateActionContext() => Player.Bind(CreateActionContext);

        protected virtual Option<IActionContext> CreateActionContext(IHumanoid player) =>
            new ActionContext(Some<IActor>(player));
    }
}
