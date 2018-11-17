using AlleyCat.Action;
using AlleyCat.Control;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public class CreateUIAction : UIAction
    {
        public PackedScene UI { get; }

        public Option<Node> Parent { get; }

        public override bool Valid => base.Valid && UI.CanInstance();

        public CreateUIAction(
            string key,
            string displayName,
            PackedScene ui,
            Option<Node> parent,
            ITriggerInput input,
            Node node,
            bool modal,
            bool active = true) : base(key, displayName, input, node, modal, active)
        {
            Ensure.That(ui, nameof(ui)).IsNotNull();

            UI = ui;
            Parent = parent;
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            Parent.IfNone(() => Scene.UIRoot).AddChild(UI.Instance());
        }
    }
}
