using AlleyCat.Autowire;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Character
{
    public class RangedMorphPanelFactory : MorphPanelFactory<RangedMorphPanel, float, RangedMorphDefinition>
    {
        [Node]
        public Option<Slider> Slider { get; set; }

        [Node]
        public Option<SpinBox> Spinner { get; set; }

        [Export, UsedImplicitly] private NodePath _slider = "../Slider";

        [Export, UsedImplicitly] private NodePath _spinner = "../Spinner";

        protected override Validation<string, RangedMorphPanel> CreateService(
            IMorph<float, RangedMorphDefinition> morph,
            Label label,
            Godot.Control node,
            ILoggerFactory loggerFactory)
        {
            return
                from slider in Slider
                    .ToValidation("Failed to find the slider.")
                from spinner in Spinner
                    .ToValidation("Failed to find the spinner.")
                select new RangedMorphPanel(morph, slider, spinner, label, node, loggerFactory);
        }
    }
}
