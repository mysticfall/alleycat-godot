using AlleyCat.Common;

namespace AlleyCat.Action
{
    public struct InteractionContext : IActionContext
    {
        public IActor Actor { get; }

        public IEntity Target { get; }

        public InteractionContext(IActor actor, IEntity target)
        {
            Actor = actor;
            Target = target;
        }
    }
}
