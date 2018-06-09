using AlleyCat.Action;

namespace AlleyCat.UI
{
    public abstract class ToggleUIAction : Action.Action
    {
        public abstract IHideableUI UI { get; }

        public override bool Valid => base.Valid && UI != null;

        protected override void DoExecute(IActor actor) => UI.Toggle();

        public override bool AllowedFor(IActor context) => true;
    }
}
