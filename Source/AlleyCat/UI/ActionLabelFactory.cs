using AlleyCat.Autowire;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class ActionLabelFactory : DelegateNodeFactory<ActionLabel, Godot.Control>
    {
        [Export]
        public bool Active { get; set; }

        [Export]
        public string Label { get; set; } = "Label";

        [Export]
        public string Action { get; set; } = "ui_accept";

        [Node]
        public Option<Label> TextLabel { get; set; }

        [Node]
        public Option<Label> ShortcutLabel { get; set; }

        [Export, UsedImplicitly] private NodePath _textLabel = "../Label";

        [Export, UsedImplicitly] private NodePath _shortcutLabel = "../Shortcut";

        protected override Validation<string, ActionLabel> CreateService(Godot.Control node,
            ILoggerFactory loggerFactory)
        {
            return
                from label in TextLabel
                    .ToValidation("Failed to find the text label.")
                from shortcutLabel in ShortcutLabel
                    .ToValidation("Failed to find the shortcut label.")
                select new ActionLabel(
                    label,
                    shortcutLabel,
                    node,
                    loggerFactory)
                {
                    Action = Action,
                    Label = Label,
                    Active = Active
                };
        }
    }
}
