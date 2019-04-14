using System.Collections.Generic;
using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class MorphGroupPanel : ScrollContainer
    {
        public IMorphGroup Group { get; private set; }

        public IEnumerable<IMorph> Morphs { get; private set; } = Seq<IMorph>();

        [Node(true)]
        protected Container MorphsPanel { get; private set; }

        [Export] private PackedScene _colorMorphPanelScene;

        [Export] private PackedScene _rangedMorphPanelScene;

        [PostConstruct]
        protected virtual void PostConstruct()
        {
            Morphs
                .Bind(m => CreateMorphPanel(m).AsEnumerable())
                .Iter(p => MorphsPanel.AddChild(p));
        }

        public void LoadGroup(IMorphGroup group, IMorphSet morphSet)
        {
            Ensure.That(group, nameof(group)).IsNotNull();
            Ensure.That(morphSet, nameof(morphSet)).IsNotNull();

            Group = group;
            Morphs = morphSet.GetMorphs(group).Filter(m => !m.Definition.Hidden);
        }

        protected virtual Option<MorphPanel> CreateMorphPanel(IMorph morph)
        {
            Ensure.That(morph, nameof(morph)).IsNotNull();

            Debug.Assert(_colorMorphPanelScene != null, "_colorMorphPanelScene != null");
            Debug.Assert(_rangedMorphPanelScene != null, "_rangedMorphPanelScene != null");

            MorphPanel panel = null;

            switch (morph)
            {
                case IMorph<float, RangedMorphDefinition> _:
                    panel = (MorphPanel) _rangedMorphPanelScene.Instance();
                    break;
                case IMorph<Color, ColorMorphDefinition> _:
                    panel = (MorphPanel) _colorMorphPanelScene.Instance();
                    break;
            }

            panel?.LoadMorph(morph);

            return panel;
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
