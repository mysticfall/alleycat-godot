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
    public class RangedMorphPanel : MorphPanel<float, RangedMorphDefinition>
    {
        public Slider Slider => _slider.Head();

        public SpinBox Spinner => _spinner.Head();

        [Node] private Option<Slider> _slider = None;

        [Node] private Option<SpinBox> _spinner = None;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Label.Text = Morph.DisplayName;

            var max = Morph.Definition.MaxValue;
            var min = Morph.Definition.MinValue;
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
