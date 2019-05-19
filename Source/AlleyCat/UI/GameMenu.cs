using System;
using AlleyCat.Control;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class GameMenu : FullScreenModalPanel
    {
        public void ShowSettings() => throw new NotImplementedException();

        public void Quit() => Node.GetTree().Quit();

        public GameMenu(
            bool pauseWhenVisible,
            Option<string> closeAction,
            IPlayerControl playerControl,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(pauseWhenVisible, closeAction, playerControl, node, loggerFactory)
        {
        }
    }
}
