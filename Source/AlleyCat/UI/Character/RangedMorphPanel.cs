using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.UI.Character.Generic;
using Godot;
using LanguageExt;

namespace AlleyCat.UI.Character
{
    public class RangedMorphPanel : MorphPanel<float, RangedMorphDefinition>
    {
        public Slider Slider => _slider.Head();

        public SpinBox Spinner => _spinner.Head();

        [Node] private Option<Slider> _slider;

        [Node] private Option<SpinBox> _spinner;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
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

            Slider.OnValueChange().Merge(Spinner.OnValueChange())
                .Select(e => e.Value)
                .Subscribe(v => Morph.Value = v)
                .AddTo(this.GetCollector());

            Morph.OnChange
                .Do(Slider.SetValue)
                .Do(Spinner.SetValue)
                .Subscribe()
                .AddTo(this.GetCollector());
        }
    }
}
