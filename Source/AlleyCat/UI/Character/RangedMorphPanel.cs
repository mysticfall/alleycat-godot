using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.UI.Character.Generic;
using Godot;

namespace AlleyCat.UI.Character
{
    public class RangedMorphPanel : MorphPanel<float, RangedMorphDefinition>
    {
        [Node]
        public Slider Slider { get; private set; }

        [Node]
        public SpinBox Spinner { get; private set; }

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

            Slider.OnValueChanged().Merge(Spinner.OnValueChanged())
                .Select(e => e.Value)
                .Subscribe(v => Morph.Value = v)
                .AddTo(this);

            Morph
                .OnChange
                .Subscribe(v =>
                {
                    Slider.Value = v;
                    Spinner.Value = v;
                })
                .AddTo(this);
        }
    }
}
