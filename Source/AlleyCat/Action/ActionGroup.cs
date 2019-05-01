using System.Collections.Generic;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Action
{
    public class ActionGroup : ActionSet, IActionGroup
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public ActionGroup(
            string key,
            string displayName,
            IEnumerable<IAction> actions,
            IEnumerable<IActionGroup> groups,
            ILoggerFactory loggerFactory) : base(actions, groups, loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(actions, nameof(actions)).IsNotNull();
            Ensure.That(groups, nameof(groups)).IsNotNull();

            Key = key;
            DisplayName = displayName;
        }
    }
}
