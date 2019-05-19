using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class MorphGroupPanel : UIControl
    {
        public IMorphGroup Group { get; }

        public IEnumerable<IMorph> Morphs { get; }

        protected Container MorphsPanel { get; }

        protected PackedScene ColorMorphPanelScene { get; }

        protected PackedScene RangedMorphPanelScene { get; }

        public MorphGroupPanel(
            IMorphGroup group,
            IEnumerable<IMorph> morphs,
            Container morphsPanel,
            PackedScene colorMorphPanelScene,
            PackedScene rangedMorphPanelScene,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(group, nameof(group)).IsNotNull();
            Ensure.That(morphs, nameof(morphs)).IsNotNull();
            Ensure.That(morphsPanel, nameof(morphsPanel)).IsNotNull();
            Ensure.That(colorMorphPanelScene, nameof(colorMorphPanelScene)).IsNotNull();
            Ensure.That(rangedMorphPanelScene, nameof(rangedMorphPanelScene)).IsNotNull();

            Group = group;
            Morphs = morphs;
            MorphsPanel = morphsPanel;
            ColorMorphPanelScene = colorMorphPanelScene;
            RangedMorphPanelScene = rangedMorphPanelScene;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Morphs
                .Bind(m => CreateMorphPanel(m))
                .Iter(p => MorphsPanel.AddChild(p));
        }

        protected virtual Option<Node> CreateMorphPanel(IMorph morph)
        {
            Ensure.That(morph, nameof(morph)).IsNotNull();

            Option<Node> node = None;

            switch (morph)
            {
                case IMorph<float, RangedMorphDefinition> _:
                    node = RangedMorphPanelScene.Instance();
                    break;
                case IMorph<Color, ColorMorphDefinition> _:
                    node = ColorMorphPanelScene.Instance();
                    break;
            }

            var factory = node.Bind(n => n.OfType<IMorphPanelFactory>());

            factory.Match(
                f => f.Morph = Some(morph),
                () => Logger.LogWarning($"Failed to find a suitable UI for morph: {morph}.")
            );

            return node;
        }
    }
}
