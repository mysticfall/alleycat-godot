using Microsoft.Extensions.Logging;

namespace AlleyCat.Action
{
    public abstract class Interaction : Action
    {
        protected Interaction(
            string key,
            string displayName,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, active, loggerFactory)
        {
        }

        protected override void DoExecute(IActionContext context)
        {
            if (context is InteractionContext iContext)
            {
                DoExecute(iContext);
            }
        }

        protected abstract void DoExecute(InteractionContext context);

        public override bool AllowedFor(IActionContext context) =>
            context is InteractionContext iContext && AllowedFor(iContext);

        protected abstract bool AllowedFor(InteractionContext context);
    }
}
