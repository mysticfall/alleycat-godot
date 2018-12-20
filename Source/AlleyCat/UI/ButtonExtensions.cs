using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public static class ButtonExtensions
    {
        private const string NodeName = "ButtonEventTracker";

        public static IObservable<ButtonPressedEvent> OnPress(this BaseButton button)
        {
            Ensure.That(button, nameof(button)).IsNotNull();

            return button.GetComponent(NodeName, _ => new ButtonEventTracker()).OnPressed;
        }
        
        public static IObservable<ButtonUpEvent> OnButtonUp(this BaseButton button)
        {
            Ensure.That(button, nameof(button)).IsNotNull();

            return button.GetComponent(NodeName, _ => new ButtonEventTracker()).OnButtonUp;
        }
        
        public static IObservable<ButtonDownEvent> OnButtonDown(this BaseButton button)
        {
            Ensure.That(button, nameof(button)).IsNotNull();

            return button.GetComponent(NodeName, _ => new ButtonEventTracker()).OnButtonDown;
        }
    }
}
