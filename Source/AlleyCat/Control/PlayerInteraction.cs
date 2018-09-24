using AlleyCat.Action;

namespace AlleyCat.Control
{
    public class PlayerInteraction : PlayerAction
    {
        protected override IActionContext CreateActionContext() =>
            new InteractionContext(Player, PlayerControl?.FocusedObject);

        protected override void DoExecute(IActionContext context) => Player.Execute(context);

        public override bool AllowedFor(IActionContext context) => 
            context?.Actor == Player && PlayerControl?.FocusedObject != null;
    }
}
