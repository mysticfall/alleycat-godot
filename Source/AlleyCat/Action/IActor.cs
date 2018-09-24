using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Action
{
    public interface IActor
    {
        IReadOnlyDictionary<string, IAction> Actions { get; }
    }

    public static class ActorExtensions
    {
        [CanBeNull]
        public static IAction FindAction(
            [NotNull] this IActor actor, 
            [NotNull] IActionContext context,
            [CanBeNull] Func<IAction, bool> predicate = null)
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));
            Ensure.Any.IsNotNull(context, nameof(context));

            return actor.Actions.Values.FirstOrDefault(
                a => a.AllowedFor(context) && (predicate == null || predicate(a)));
        }

        public static void Execute(
            [NotNull] this IActor actor, 
            [NotNull] IActionContext context,
            [CanBeNull] Func<IAction, bool> predicate = null) =>
            actor.FindAction(context, predicate)?.Execute(context);
    }
}
