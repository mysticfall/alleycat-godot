using AlleyCat.Autowire;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Character
{
    public class MorphListPanelFactory : DelegateNodeFactory<MorphListPanel, Godot.Control>
    {
        [Node]
        public Option<TabContainer> TabContainer { get; set; }

        [Export]
        public PackedScene GroupPanelScene { get; set; }

        [Export, UsedImplicitly] private NodePath _tabContainer = "../Tab Container";

        protected override Validation<string, MorphListPanel> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from tabContainer in TabContainer
                    .ToValidation("Failed to find the tab container.")
                from scene in Optional(GroupPanelScene)
                    .ToValidation("Missing group panel scene.")
                select new MorphListPanel(tabContainer, scene, node, loggerFactory);
        }
    }
}
