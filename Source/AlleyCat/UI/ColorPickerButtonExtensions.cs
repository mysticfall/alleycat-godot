using System;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.UI
{
    public static class ColorPickerButtonExtensions
    {
        private const string NodeName = "ColorPickerButtonEventTracker";

        public static IObservable<ColorChangedEvent> OnColorChange(this ColorPickerButton button) =>
            button.GetComponent(NodeName, _ => new ColorPickerButtonEventTracker()).OnColorChange;
    }
}
