using AlleyCat.Action;
using AlleyCat.Common;
using AlleyCat.Control;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public class ToggleUIAction : UIAction
    {
        public Option<IHideableUI> UI => Scene.Root.FindComponent<IHideableUI>(_ui);

        public override bool Valid => base.Valid && UI.IsSome;

        private readonly NodePath _ui;

        public ToggleUIAction(
            string key,
            string displayName,
            NodePath ui,
            ITriggerInput input,
            Node node,
            bool modal,
            bool active = true) : base(key, displayName, input, node, modal, active)
        {
            Ensure.That(ui, nameof(ui)).IsNotNull();

            _ui = ui;
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            UI.Iter(ui => ui.Toggle());
        }
    }
}
