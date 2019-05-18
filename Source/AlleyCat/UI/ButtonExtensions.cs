using System;
using AlleyCat.Event;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public static class ButtonExtensions
    {
        public static IObservable<Unit> OnPress(this BaseButton button)
        {
            return button.FromSignal("pressed").AsUnitObservable();
        }

        public static IObservable<Unit> OnButtonUp(this BaseButton button)
        {
            return button.FromSignal("button_up").AsUnitObservable();
        }

        public static IObservable<Unit> OnButtonDown(this BaseButton button)
        {
            return button.FromSignal("button_down").AsUnitObservable();
        }
    }
}
