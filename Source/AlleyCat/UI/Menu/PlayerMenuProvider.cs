using System.Linq;
using AlleyCat.Action;
using AlleyCat.Control;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public abstract class PlayerMenuProvider : GameNode, IMenuModel
    {
        public string Key { get; }

        public string DisplayName { get; }

        public object Model => this;

        public Option<IMenuModel> Parent => None;

        protected PlayerControl PlayerControl { get; }

        protected Option<IActor> Actor => PlayerControl.Character.OfType<IActor>().HeadOrNone();

        protected PlayerMenuProvider(
            string key,
            string displayName,
            PlayerControl playerControl,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(displayName, nameof(displayName)).IsNotNull();
            Ensure.That(playerControl, nameof(playerControl)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            PlayerControl = playerControl;
        }
    }
}
