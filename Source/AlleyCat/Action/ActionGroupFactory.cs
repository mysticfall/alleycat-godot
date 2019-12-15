using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Action
{
    [AutowireContext]
    public class ActionGroupFactory : GameNodeFactory<ActionGroup>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Service(local: true)]
        public IEnumerable<IAction> Actions { get; set; } = Seq<IAction>();

        [Service(local: true)]
        public IEnumerable<IActionGroup> Groups { get; set; } = Seq<IActionGroup>();

        protected override Validation<string, ActionGroup> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return new ActionGroup(key, displayName, Actions, Groups, loggerFactory);
        }
    }
}
