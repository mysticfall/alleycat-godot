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
        public static IAction FindAction([NotNull] this IActor actor, [NotNull] IActionContext context)
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));
            Ensure.Any.IsNotNull(context, nameof(context));

            return actor.Actions.Values.FirstOrDefault(a => a.AllowedFor(context));
        }

        public static void Execute([NotNull] this IActor actor, [NotNull] IActionContext context) =>
            actor.FindAction(context)?.Execute(context);
    }
}
