using AlleyCat.Action;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class ToggleUIAction : UIAction
    {
        public IHideableUI UI => _uiNode.Head();

        public override bool Valid => base.Valid && _uiNode.IsSome;

        [Export] private NodePath _ui;

        private Option<IHideableUI> _uiNode;

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            UI.Toggle();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _uiNode = Optional(_ui).Bind(this.FindComponent<IHideableUI>);
        }
    }
}
