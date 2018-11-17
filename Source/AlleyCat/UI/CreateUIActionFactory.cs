using AlleyCat.Common;
using AlleyCat.Control;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class CreateUIActionFactory : UIActionFactory<CreateUIAction>
    {
        [Export]
        public PackedScene UI { get; set; }

        [Export]
        public NodePath Parent { get; set; }

        protected override Validation<string, CreateUIAction> CreateService(
            string key, string displayName, ITriggerInput input)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(input, nameof(input)).IsNotNull();

            var parent = Optional(Parent).Bind(this.FindComponent<Node>);

            return
                from ui in Optional(UI).ToValidation("Missing the target UI.")
                select new CreateUIAction(
                    key,
                    displayName,
                    ui,
                    parent,
                    input,
                    this,
                    Modal,
                    Active);
        }
    }
}
