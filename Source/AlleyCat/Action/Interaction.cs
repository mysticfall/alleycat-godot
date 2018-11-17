using EnsureThat;

namespace AlleyCat.Action
{
    public abstract class Interaction : Action
    {
        protected Interaction(
            string key, 
            string displayName, 
            bool active = true) : base(key, displayName, active)
        {
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            if (context is InteractionContext iContext)
            {
                DoExecute(iContext);
            }
        }

        protected abstract void DoExecute(InteractionContext context);

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context is InteractionContext iContext && AllowedFor(iContext);
        }

        protected abstract bool AllowedFor(InteractionContext context);
    }
}
