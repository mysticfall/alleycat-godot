using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public static class ButtonExtensions
    {
        private const string NodeName = "ButtonEventTracker";

        [NotNull]
        public static IObservable<ButtonPressedEvent> OnPress([NotNull] this Button button)
        {
            Ensure.Any.IsNotNull(button, nameof(button));

            var tracker = button.GetOrCreateNode(NodeName, _ => new ButtonEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnPressed;
        }
    }
}
