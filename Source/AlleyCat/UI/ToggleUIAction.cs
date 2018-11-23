using AlleyCat.Action;
using AlleyCat.Common;
using AlleyCat.Control;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class ToggleUIAction : UIAction
    {
        public Option<IHideableUI> UI => Scene.Bind(s => s.Root.FindComponent<IHideableUI>(_ui));

        public override bool Valid => base.Valid && UI.IsSome;

        private readonly NodePath _ui;

        public ToggleUIAction(
            string key,
            string displayName,
            NodePath ui,
            ITriggerInput input,
            Node node,
            bool modal,
            bool active,
            ILogger logger) : base(key, displayName, input, node, modal, active, logger)
        {
            Ensure.That(ui, nameof(ui)).IsNotNull();

            _ui = ui;
        }

        protected override void DoExecute(IActionContext context) => UI.Iter(ui => ui.Toggle());
    }
}
