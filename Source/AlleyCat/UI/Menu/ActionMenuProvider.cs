using System.Collections.Generic;
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
    public class ActionMenuProvider : PlayerMenuProvider, IMenuStructureProvider, IMenuHandler
    {
        public ActionMenuProvider(
            string key,
            string displayName,
            PlayerControl playerControl,
            ILoggerFactory loggerFactory) : base(key, displayName, playerControl, loggerFactory)
        {
        }

        public bool HasChildren(object item) => item == this || item is IActionSet;

        public IEnumerable<object> FindChildren(object item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            IEnumerable<object> FromActor(IActor actor)
            {
                var set = actor.Actions;

                return set.Groups.Concat<object>(set.Actions);
            }

            switch (item)
            {
                case ActionMenuProvider provider when provider == this:
                    return PlayerControl.Character.Bind(FromActor);
                case IActionSet set:
                    return set.Groups.Concat<object>(set.Actions);
                case IActor actor:
                    return FromActor(actor);
                default:
                    return Enumerable.Empty<object>();
            }
        }

        protected virtual Option<IActionContext> CreateActionContext(IMenuModel item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            switch (item.Model)
            {
                case Interaction _:

                    Option<IEntity> FindTarget(IMenuModel i) =>
                        i.Model is IEntity entity ? Some(entity) : i.Parent.Bind(FindTarget);

                    var target = FindTarget(item);

                    var context =
                        from a in Actor
                        from t in target
                        select new InteractionContext(a, t);

                    return context.OfType<IActionContext>().HeadOrNone();
                case IAction _:
                    return Some<IActionContext>(new ActionContext(Actor));
                default:
                    return None;
            }
        }

        public bool CanExecute(IMenuModel item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            return item.Model is IAction action && CreateActionContext(item).Exists(action.AllowedFor);
        }

        public void Execute(IMenuModel item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            if (item.Model is IAction action)
            {
                CreateActionContext(item).Iter(action.Execute);
            }
        }
    }
}
