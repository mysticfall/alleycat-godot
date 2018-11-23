using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.UI.Character.Generic;
using Godot;

namespace AlleyCat.UI.Character
{
    public class ColorMorphPanel : MorphPanel<Color, ColorMorphDefinition>
    {
        [Node(true)]
        protected ColorPickerButton Button { get; private set; }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Label.Text = Morph.DisplayName;

            Button.EditAlpha = Morph.Definition.UseAlpha;

            Button.OnColorChange()
                .Select(e => e.Color)
                .Select(v => Morph.Definition.UseAlpha ? v : ToOpaqueColor(v))
                .Subscribe(v => Morph.Value = v)
                .DisposeWith(this);

            Morph
                .OnChange
                .Subscribe(v => Button.Color = v)
                .DisposeWith(this);
        }

        private Color ToOpaqueColor(Color color)
        {
            color.a = Morph.Definition.Default.a;
            return color;
        }
    }
}
