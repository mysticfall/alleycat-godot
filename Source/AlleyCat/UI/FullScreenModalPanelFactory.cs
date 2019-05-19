using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public abstract class FullScreenModalPanelFactory<TObj, TDel> : DelegateObjectFactory<TObj, TDel>
        where TObj : IDelegateObject<TDel>
        where TDel : Node
    {
        [Export]
        public bool PauseWhenVisible { get; set; } = true;

        [Export]
        public string CloseAction { get; set; } = "ui_cancel";

        [Service]
        public Option<IPlayerControl> PlayerControl { get; set; }

        protected override Validation<string, TObj> CreateService(TDel node, ILoggerFactory loggerFactory)
        {
            var service = 
                from playerControl in PlayerControl
                    .ToValidation("Failed to find the player control.")
                select CreateService(
                    CloseAction.TrimToOption(),
                    playerControl,
                    node,
                    loggerFactory);

            return service.Bind(identity);
        }

        protected abstract Validation<string, TObj> CreateService(
            Option<string> closeAction,
            IPlayerControl playerControl,
            TDel node,
            ILoggerFactory loggerFactory);
    }
}
