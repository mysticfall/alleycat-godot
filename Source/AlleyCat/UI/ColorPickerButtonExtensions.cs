using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public static class ColorPickerButtonExtensions
    {
        public static IObservable<Color> OnColorChange(this ColorPickerButton button)
        {
            return button.FromSignal("color_changed")
                .SelectMany(args => args.HeadOrNone().OfType<Color>().ToObservable());
        }
    }
}
