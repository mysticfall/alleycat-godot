using AlleyCat.Control;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class ToggleUIActionFactory : UIActionFactory<ToggleUIAction>
    {
        [Export]
        public NodePath UI { get; set; }

        protected override Validation<string, ToggleUIAction> CreateService(
            string key, string displayName, ITriggerInput input)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(input, nameof(input)).IsNotNull();

            return
                from ui in Optional(UI)
                    .ToValidation("Missing the target UI.")
                select new ToggleUIAction(
                    key,
                    displayName,
                    ui,
                    input,
                    this,
                    Modal,
                    Active);
        }
    }
}
