using AlleyCat.Autowire;
using AlleyCat.Game;
using AlleyCat.Morph;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class MorphGroupPanelFactory : DelegateObjectFactory<MorphGroupPanel, Godot.Control>
    {
        public Option<IMorphGroup> Group { get; set; }

        public Option<IMorphSet> Morphs { get; set; }

        [Node]
        public Option<Container> MorphsPanel { get; set; }

        [Export]
        public PackedScene ColorMorphPanelScene { get; set; }

        [Export]
        public PackedScene RangedMorphPanelScene { get; set; }

        [Export, UsedImplicitly] private NodePath _morphsPanel = "../MorphsPanel";

        protected override Validation<string, MorphGroupPanel> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from morphGroup in Group
                    .ToValidation("Failed to find the morph group.")
                from morphsPanel in MorphsPanel
                    .ToValidation("Failed to find the morph list panel.")
                from colorMorphScene in Optional(ColorMorphPanelScene)
                    .ToValidation("Missing color morph panel scene.")
                from rangedMorphScene in Optional(RangedMorphPanelScene)
                    .ToValidation("Missing ranged morph panel scene.")
                select new MorphGroupPanel(
                    morphGroup,
                    Morphs.Bind(m => m.GetMorphs(morphGroup)).Filter(m => !m.Definition.Hidden),
                    morphsPanel,
                    colorMorphScene,
                    rangedMorphScene,
                    node,
                    loggerFactory);
        }
    }
}
