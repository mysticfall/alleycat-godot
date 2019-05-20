using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Menu
{
    public class ActionMenuProviderFactory : GameObjectFactory<ActionMenuProvider>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Service]
        public Option<PlayerControl> PlayerControl { get; set; }

        protected override Validation<string, ActionMenuProvider> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return
                from control in PlayerControl
                    .ToValidation("Failed to find the player control.")
                select new ActionMenuProvider(key, displayName, control, loggerFactory);
        }
    }
}
