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
    public class RangedMorphPanel : MorphPanel<float, RangedMorphDefinition>
    {
        [Node(true)] 
        protected Slider Slider { get; private set; }

        [Node(true)] 
        protected SpinBox Spinner { get; private set; }

        [PostConstruct]
        protected virtual void PostConstruct()
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

            var onDispose = this.OnDispose().Where(identity);

            Slider.OnValueChange().Merge(Spinner.OnValueChange())
                .TakeUntil(onDispose)
                .Subscribe(v => Morph.Value = v, this);

            Morph.OnChange
                .Do(Slider.SetValue)
                .Do(Spinner.SetValue)
                .TakeUntil(onDispose)
                .Subscribe(this);
        }
    }
}
