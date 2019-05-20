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
    public class RangedMorphPanel : MorphPanel<float, RangedMorphDefinition>
    {
        protected Slider Slider { get; }

        protected SpinBox Spinner { get; }

        public RangedMorphPanel(
            IMorph<float, RangedMorphDefinition> morph,
            Slider slider,
            SpinBox spinner,
            Label label,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(morph, label, node, loggerFactory)
        {
            Ensure.That(slider, nameof(slider)).IsNotNull();
            Ensure.That(spinner, nameof(spinner)).IsNotNull();

            Slider = slider;
            Spinner = spinner;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Label.Text = Morph.DisplayName;

            var max = Morph.Definition.Range.Max;
            var min = Morph.Definition.Range.Min;
            var value = Morph.Definition.Default;

            Slider.MinValue = min;
            Slider.MaxValue = max;
            Slider.Value = value;

            Spinner.MinValue = min;
            Spinner.MaxValue = max;
            Spinner.Value = value;
            Spinner.Step = (max - min) / 100;

            var disposed = Disposed.Where(identity);

            Slider.OnValueChange().Merge(Spinner.OnValueChange())
                .TakeUntil(disposed)
                .Subscribe(v => Morph.Value = v, this);

            Morph.OnChange
                .Do(v => Slider.Value = v)
                .Do(v => Spinner.Value = v)
                .TakeUntil(disposed)
                .Subscribe(this);
        }
    }
}
