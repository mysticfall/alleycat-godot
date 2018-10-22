using System;
using AlleyCat.Action;
using AlleyCat.Autowire;
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

        protected IPlayerControl PlayerControl => _playerControl.Head();

        [Ancestor] private Option<IPlayerControl> _playerControl = None;

        protected override void OnInitialize()
        {
            base.OnInitialize();

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
