using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    [AutowireContext]
    public class MenuFactory : DelegateNodeFactory<Menu, Godot.Control>
    {
        [Service(local: true)]
        public IEnumerable<IMenuModel> RootItems { get; set; }

        [Service]
        public IEnumerable<IMenuRenderer> Renderers { get; set; }

        [Service]
        public IEnumerable<IMenuStructureProvider> StructureProviders { get; set; }

        [Service]
        public IEnumerable<IMenuHandler> MenuHandlers { get; set; }

        [Node]
        public Option<Label> Breadcrumb { get; set; }

        [Node]
        public Option<ActionLabel> UpLabel { get; set; }

        [Node]
        public Option<ActionLabel> CloseLabel { get; set; }

        [Node]
        public Option<Label> EmptyLabel { get; set; }

        [Node]
        public Option<Node> ItemsContainer { get; set; }

        [Export]
        public PackedScene ItemScene { get; set; }

        [Export]
        public string BackAction { get; set; } = "ui_back";

        [Export, UsedImplicitly] private NodePath _breadcrumb = "../Breadcrumb";

        [Export, UsedImplicitly] private NodePath _itemsContainer = "../Items";

        [Export, UsedImplicitly] private NodePath _upLabel = "../Up";

        [Export, UsedImplicitly] private NodePath _closeLabel = "../Close";

        [Export, UsedImplicitly] private NodePath _emptyLabel = "../Items/Empty Label";

        protected override Validation<string, Menu> CreateService(Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from rootItems in Optional(RootItems).Filter(Enumerable.Any)
                    .ToValidation("Menu must have at least one root item.")
                from menuHandlers in Optional(MenuHandlers).Filter(Enumerable.Any)
                    .ToValidation("Menu must have at least menu handler.")
                from itemsContainer in ItemsContainer
                    .ToValidation("Failed to find the items container.")
                from itemScene in Optional(ItemScene)
                    .ToValidation("Item scene is missing.")
                select new Menu(
                    rootItems,
                    menuHandlers,
                    Optional(StructureProviders).Flatten(),
                    Optional(Renderers).Flatten(),
                    BackAction.TrimToOption(),
                    node,
                    itemsContainer,
                    CloseLabel,
                    UpLabel,
                    EmptyLabel,
                    Breadcrumb,
                    itemScene,
                    loggerFactory);
        }
    }
}
