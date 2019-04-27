using System.Collections.Generic;
using AlleyCat.Game;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Action
{
    public class ActionGroup : GameObject, IActionGroup
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public IEnumerable<IActionGroup> Groups { get; }

        public IEnumerable<IAction> Actions { get; }

        public ActionGroup(
            string key,
            string displayName,
            IEnumerable<IActionGroup> groups,
            IEnumerable<IAction> actions,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(groups, nameof(groups)).IsNotNull();
            Ensure.That(actions, nameof(actions)).IsNotNull();

            Key = key;
            DisplayName = displayName;

            Groups = groups.Freeze(); 
            Actions = actions.Freeze();
        }
    }
}
