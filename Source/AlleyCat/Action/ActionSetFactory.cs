using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Game;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Action
{
    [AutowireContext]
    public class ActionSetFactory : GameNodeFactory<ActionSet>
    {
        [Service(local: true)]
        public IEnumerable<IActionGroup> Groups { get; set; } = Seq<IActionGroup>();

        [Service(local: true)]
        public IEnumerable<IAction> Actions { get; set; } = Seq<IAction>();

        protected override Validation<string, ActionSet> CreateService(ILoggerFactory loggerFactory)
        {
            return new ActionSet(Actions, Groups, loggerFactory);
        }
    }
}
