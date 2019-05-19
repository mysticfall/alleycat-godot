using AlleyCat.Autowire;
using AlleyCat.Control;
using AlleyCat.View;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Inventory
{
    public class InventoryViewFactory : FullScreenModalPanelFactory<InventoryView, Godot.Control>
    {
        [Node]
        public Option<InspectingView> ViewControl { get; set; }

        [Node]
        public Option<Tree> Tree { get; set; }

        [Node]
        public Option<Container> Buttons { get; set; }

        [Node]
        public Option<MeshInstance> ItemStand { get; set; }

        [Node]
        public Option<Panel> InfoPanel { get; set; }

        [Node]
        public Option<Label> Title { get; set; }

        [Node]
        public Option<Label> Type { get; set; }

        [Node]
        public Option<RichTextLabel> Description { get; set; }

        [Export]
        public PackedScene ActionButton { get; set; }

        [Export, UsedImplicitly] private NodePath _viewControl = "../Control/View";

        [Export, UsedImplicitly] private NodePath _tree = "../List Panel/Layout/Tree";

        [Export, UsedImplicitly] private NodePath _buttons = "../List Panel/Layout/Buttons Panel";

        [Export, UsedImplicitly] private NodePath _itemStand = "../Content Panel/Viewport/Item Box/Item";

        [Export, UsedImplicitly] private NodePath _infoPanel = "../Content Panel/Info Panel";

        [Export, UsedImplicitly] private NodePath _title = "../Content Panel/Info Panel/Title";

        [Export, UsedImplicitly] private NodePath _type = "../Content Panel/Info Panel/Type";

        [Export, UsedImplicitly] private NodePath _description = "../Content Panel/Info Panel/Description";

        protected override Validation<string, InventoryView> CreateService(
            Option<string> closeAction,
            IPlayerControl playerControl,
            Godot.Control node,
            ILoggerFactory loggerFactory)
        {
            return
                from viewControl in ViewControl
                    .ToValidation("Failed to find the view control.")
                from itemStand in ItemStand
                    .ToValidation("Failed to find the item stand.")
                from tree in Tree
                    .ToValidation("Failed to find the tree view.")
                from buttonContainer in Buttons
                    .ToValidation("Failed to find the button container.")
                from infoPanel in InfoPanel
                    .ToValidation("Failed to find the information panel.")
                from titleLabel in Title
                    .ToValidation("Failed to find the title label.")
                from actionButton in Optional(ActionButton)
                    .ToValidation("Failed to find action button template.")
                select new InventoryView(
                    playerControl,
                    viewControl,
                    itemStand,
                    PauseWhenVisible,
                    closeAction,
                    node,
                    tree,
                    buttonContainer,
                    infoPanel,
                    titleLabel,
                    Type,
                    Description,
                    actionButton,
                    loggerFactory);
        }
    }
}
