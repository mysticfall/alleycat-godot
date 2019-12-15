using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class EntityLabelFactory : DelegateNodeFactory<EntityLabel, Godot.Control>
    {
        [Export]
        public string InteractAction { get; set; } = "interact";

        [Node]
        public Option<Label> Title { get; set; }

        [Node]
        public Option<Godot.Control> ActionPanel { get; set; }

        [Node]
        public Option<Label> Shortcut { get; set; }

        [Node]
        public Option<Label> ActionTitle { get; set; }

        [Service]
        public Option<IPlayerControl> PlayerControl { get; set; }

        [Export, UsedImplicitly] private NodePath _title = "../Container/Title";

        [Export, UsedImplicitly] private NodePath _actionPanel = "../Container/Action";

        [Export, UsedImplicitly] private NodePath _shortcut = "../Container/Action/Shortcut";

        [Export, UsedImplicitly] private NodePath _actionTitle = "../Container/Action/Action Title";

        protected override Validation<string, EntityLabel> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from playerControl in PlayerControl
                    .ToValidation("Failed to find the player control.")
                from interactAction in InteractAction.TrimToOption()
                    .ToValidation("Interact action was not specified.")
                from titleLabel in Title
                    .ToValidation("Failed to find the title label.")
                select new EntityLabel(
                    playerControl,
                    interactAction,
                    titleLabel,
                    Shortcut,
                    ActionTitle,
                    ActionPanel,
                    this,
                    node,
                    loggerFactory);
        }
    }
}
