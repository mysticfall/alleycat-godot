using AlleyCat.Autowire;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Menu
{
    public class MenuItemFactory : DelegateNodeFactory<MenuItem, Godot.Control>
    {
        [Ancestor]
        public Option<IMenu> Parent { get; set; }

        [Node]
        public Option<Label> Label { get; set; }

        [Node]
        public Option<Label> ShortcutLabel { get; set; }

        [Export, UsedImplicitly] private NodePath _label = "../Label";

        [Export, UsedImplicitly] private NodePath _shortcutLabel = "../Shortcut";

        protected override Validation<string, MenuItem> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from parent in Parent
                    .ToValidation("Failed to find the menu parent.")
                from label in Label
                    .ToValidation("Failed to find the label.")
                select new MenuItem(
                    parent,
                    label,
                    ShortcutLabel,
                    node,
                    loggerFactory);
        }
    }
}
