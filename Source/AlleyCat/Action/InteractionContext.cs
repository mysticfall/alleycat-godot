using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Action
{
    public struct InteractionContext : IActionContext
    {
        public Option<IActor> Actor { get; }

        public IEntity Target { get; }

        public InteractionContext(IActor actor, IEntity target)
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();
            Ensure.That(target, nameof(target)).IsNotNull();

            Actor = Some(actor);
            Target = target;
        }

        public override string ToString() => $"InteractionContext[Actor = {Actor}, Target = {Target}]";
    }
}
