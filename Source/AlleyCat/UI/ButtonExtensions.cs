using System;
using System.Reactive;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public static class ButtonExtensions
    {
        public static IObservable<Unit> OnPress(this BaseButton button)
        {
            return button.FromSignal("pressed").Select(_ => Unit.Default);
        }

        public static IObservable<Unit> OnButtonUp(this BaseButton button)
        {
            return button.FromSignal("button_up").Select(_ => Unit.Default);
        }

        public static IObservable<Unit> OnButtonDown(this BaseButton button)
        {
            return button.FromSignal("button_down").Select(_ => Unit.Default);
        }
    }
}
