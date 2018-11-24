using AlleyCat.Common;
using AlleyCat.Control;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
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
            string key, string displayName, ITriggerInput input, ILoggerFactory loggerFactory)
        {
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
                    Active,
                    loggerFactory);
        }
    }
}
