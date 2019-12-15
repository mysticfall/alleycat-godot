using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Action
{
    public class ActionSet : GameNode, IActionSet
    {
        public IEnumerator<KeyValuePair<string, IAction>> GetEnumerator() =>
            _actions.Map(p => new KeyValuePair<string, IAction>(p.Item1, p.Item2)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _actions.Count;

        public bool ContainsKey(string key) => _actions.ContainsKey(key);

        public bool TryGetValue(string key, out IAction value)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            if (!_actions.ContainsKey(key))
            {
                value = null;
                return false;
            }

            value = _actions[key];
            return true;
        }

        public IAction this[string key] => _actions[key];

        public IEnumerable<string> Keys => _actions.Keys;

        public IEnumerable<IAction> Values => _actions.Values;

        public IEnumerable<IAction> Actions { get; }

        public IEnumerable<IActionGroup> Groups { get; }

        private readonly Map<string, IAction> _actions;

        public ActionSet(
            IEnumerable<IAction> actions,
            IEnumerable<IActionGroup> groups,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(actions, nameof(actions)).IsNotNull();
            Ensure.That(groups, nameof(groups)).IsNotNull();

            Actions = actions.Freeze();
            Groups = groups.Freeze();

            _actions = Groups.Fold(Actions, (a, g) => a.Concat(g.Values)).ToMap();
        }
    }
}
