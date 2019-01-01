using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Event;
using AlleyCat.Logging;
using AlleyCat.UI.Character.Generic;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class ColorMorphPanel : MorphPanel<Color, ColorMorphDefinition>
    {
        [Node(true)]
        protected ColorPickerButton Button { get; private set; }

        [PostConstruct]
        protected virtual void PostConstruct()
        {
            Label.Text = Morph.DisplayName;

            Button.EditAlpha = Morph.Definition.UseAlpha;

            var onDispose = this.OnDispose().Where(identity);

            Button.OnColorChange()
                .Select(e => e.Color)
                .Select(v => Morph.Definition.UseAlpha ? v : ToOpaqueColor(v))
                .TakeUntil(onDispose)
                .Subscribe(v => Morph.Value = v, this);

            Morph.OnChange
                .TakeUntil(onDispose)
                .Subscribe(v => Button.Color = v, this);
        }

        private Color ToOpaqueColor(Color color)
        {
            color.a = Morph.Definition.Default.a;
            return color;
        }
    }
}
