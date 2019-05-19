using System.Reactive.Linq;
using AlleyCat.Logging;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using AlleyCat.UI.Character.Generic;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class ColorMorphPanel : MorphPanel<Color, ColorMorphDefinition>
    {
        protected ColorPickerButton Button { get; }

        public ColorMorphPanel(
            IMorph<Color, ColorMorphDefinition> morph,
            ColorPickerButton button,
            Label label,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(morph, label, node, loggerFactory)
        {
            Ensure.That(button, nameof(button)).IsNotNull();

            Button = button;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Label.Text = Morph.DisplayName;

            Button.EditAlpha = Morph.Definition.UseAlpha;

            var disposed = Disposed.Where(identity);

            Button.OnColorChange()
                .Select(v => Morph.Definition.UseAlpha ? v : ToOpaqueColor(v))
                .TakeUntil(disposed)
                .Subscribe(v => Morph.Value = v, this);

            Morph.OnChange
                .TakeUntil(disposed)
                .Subscribe(v => Button.Color = v, this);
        }

        private Color ToOpaqueColor(Color color)
        {
            color.a = Morph.Definition.Default.a;
            return color;
        }
    }
}
