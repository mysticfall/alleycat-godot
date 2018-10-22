using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public static class ButtonExtensions
    {
        private const string NodeName = "ButtonEventTracker";

        public static IObservable<ButtonPressedEvent> OnPress(this Button button)
        {
            Ensure.That(button, nameof(button)).IsNotNull();

            return button.GetComponent(NodeName, _ => new ButtonEventTracker()).OnPressed;
        }
    }
}
