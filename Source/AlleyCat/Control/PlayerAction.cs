using System;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    public abstract class PlayerAction : InputAction
    {
        protected IHumanoid Player => PlayerControl?.Character;

        [Service]
        protected IPlayerControl PlayerControl { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            PlayerControl.OnActiveStateChange
                .Subscribe(v => Active = v)
                .AddTo(this);
        }

        protected override IActionContext CreateActionContext() => new ActionContext(Player);
    }
}
