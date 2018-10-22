using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.UI.Character.Generic;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class ColorMorphPanel : MorphPanel<Color, ColorMorphDefinition>
    {
        public ColorPickerButton Button => _button.Head();

        [Node] private Option<ColorPickerButton> _button = None;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Label.Text = Morph.DisplayName;

            Button.EditAlpha = Morph.Definition.UseAlpha;

            Button.OnColorChange()
                .Select(e => e.Color)
                .Select(v => Morph.Definition.UseAlpha ? v : ToOpaqueColor(v))
                .Subscribe(v => Morph.Value = v)
                .AddTo(this.GetCollector());

            Morph
                .OnChange
                .Subscribe(v => Button.Color = v)
                .AddTo(this.GetCollector());
        }

        private Color ToOpaqueColor(Color color)
        {
            color.a = Morph.Definition.Default.a;
            return color;
        }
    }
}
