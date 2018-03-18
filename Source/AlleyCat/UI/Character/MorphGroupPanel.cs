using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Character.Morph.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Character
{
    public class MorphGroupPanel : ScrollContainer
    {
        public IMorphGroup Group { get; private set; }

        public IEnumerable<IMorph> Morphs { get; private set; }

        [Node]
        protected Container MorphsPanel { get; private set; }

        [Export, UsedImplicitly] private PackedScene _rangedMorphPanelScene;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Morphs
                .Select(CreateMorphPanel)
                .Where(p => p != null)
                .ToList()
                .ForEach(p => MorphsPanel.AddChild(p));
        }

        public void LoadGroup([NotNull] IMorphGroup group, [NotNull] IEnumerable<IMorph> morphs)
        {
            Ensure.Any.IsNotNull(group, nameof(group));
            Ensure.Any.IsNotNull(morphs, nameof(morphs));

            Group = group;
            Morphs = morphs;
        }

        protected virtual MorphPanel CreateMorphPanel([NotNull] IMorph morph)
        {
            Ensure.Any.IsNotNull(morph, nameof(morph));

            MorphPanel panel = null;

            if (morph is IMorph<float, RangedMorphDefinition> rangedMorph)
            {
                panel = (MorphPanel) _rangedMorphPanelScene.Instance();
                panel.LoadMorph(rangedMorph);
            }

            return panel;
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
