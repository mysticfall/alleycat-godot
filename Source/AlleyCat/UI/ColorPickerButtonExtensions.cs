using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public static class ColorPickerButtonExtensions
    {
        private const string NodeName = "ColorPickerButtonEventTracker";

        [NotNull]
        public static IObservable<ColorChangedEvent> OnColorChange(
            [NotNull] this ColorPickerButton button)
        {
            Ensure.Any.IsNotNull(button, nameof(button));

            var tracker = button.GetOrCreateNode(NodeName, _ => new ColorPickerButtonEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnColorChange;
        }
    }
}
