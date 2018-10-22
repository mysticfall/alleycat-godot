using System;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Action
{
    public interface IActor
    {
        Map<string, IAction> Actions { get; }
    }

    public static class ActorExtensions
    {
        public static Option<IAction> FindAction(this IActor actor, IActionContext context)
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            return actor.Actions.Values.Find(a => a.AllowedFor(context));
        }

        public static Option<IAction> FindAction(
            this IActor actor, IActionContext context, Func<IAction, bool> predicate)
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();
            Ensure.That(predicate, nameof(predicate)).IsNotNull();

            return actor.Actions.Values.Find(a => a.AllowedFor(context) && predicate(a));
        }

        public static void Execute(this IActor actor, IActionContext context) =>
            actor.FindAction(context).Iter(a => a.Execute(context));

        public static void Execute(
            this IActor actor, IActionContext context, Func<IAction, bool> predicate) =>
            actor.FindAction(context, predicate).Iter(a => a.Execute(context));
    }
}
