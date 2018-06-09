using System.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;

namespace AlleyCat.Control
{
    public class InteractAction : Action.Action
    {
        [Ancestor]
        public IPlayerControl FocusTracker { get; private set; }

        protected override void DoExecute(IActor actor)
        {
            (FocusTracker?.FocusedObject as IInteractable)?
                .Actions
                .FirstOrDefault(a => a.AllowedFor(actor))?
                .Execute(actor);
        }

        public override bool AllowedFor(IActor context)
        {
            return true;
        }
    }
}
