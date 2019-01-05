using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public static class ButtonExtensions
    {
        public static IObservable<ButtonPressedEvent> OnPress(this BaseButton button)
        {
            return button.FromSignal("pressed").Select(_ => new ButtonPressedEvent(button));
        }

        public static IObservable<ButtonUpEvent> OnButtonUp(this BaseButton button)
        {
            return button.FromSignal("button_up").Select(_ => new ButtonUpEvent(button));
        }

        public static IObservable<ButtonDownEvent> OnButtonDown(this BaseButton button)
        {
            return button.FromSignal("button_down").Select(_ => new ButtonDownEvent(button));
        }
    }
}
