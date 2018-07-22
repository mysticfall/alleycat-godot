using AlleyCat.Action;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class ToggleUIAction : UIAction
    {
        public IHideableUI UI => this.GetNodeOrDefault<IHideableUI>(_ui);

        public override bool Valid => base.Valid && UI != null;

        [Export, UsedImplicitly] private NodePath _ui;

        protected override void DoExecute(IActor actor) => UI?.Toggle();
    }
}
