using AlleyCat.Autowire;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Character
{
    public class ColorMorphPanelFactory : MorphPanelFactory<ColorMorphPanel, Color, ColorMorphDefinition>
    {
        [Node]
        public Option<ColorPickerButton> Button { get; set; }

        [Export, UsedImplicitly] private NodePath _button = "../Button";

        protected override Validation<string, ColorMorphPanel> CreateService(
            IMorph<Color, ColorMorphDefinition> morph,
            Label label,
            Godot.Control node,
            ILoggerFactory loggerFactory)
        {
            return
                from button in Button
                    .ToValidation("Failed to find the button.")
                select new ColorMorphPanel(morph, button, label, node, loggerFactory);
        }
    }
}
