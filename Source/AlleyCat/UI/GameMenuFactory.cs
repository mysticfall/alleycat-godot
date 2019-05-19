using AlleyCat.Control;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class GameMenuFactory : FullScreenModalPanelFactory<GameMenu, Godot.Control>
    {
        public void Resume() => Service.Iter(s => s.Resume());

        public void Quit() => Service.Iter(s => s.Quit());

        [UsedImplicitly]
        public void ShowSettings() => Service.Iter(s => s.ShowSettings());

        protected override Validation<string, GameMenu> CreateService(
            Option<string> closeAction,
            IPlayerControl playerControl,
            Godot.Control node,
            ILoggerFactory loggerFactory)
        {
            return new GameMenu(PauseWhenVisible, closeAction, playerControl, node, loggerFactory);
        }
    }
}
