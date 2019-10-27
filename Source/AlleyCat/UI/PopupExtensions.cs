using System;
using AlleyCat.Event;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public static class PopupExtensions
    {
        public static IObservable<Unit> OnHide(this Popup popup)
        {
            return popup.FromSignal("popup_hide").AsUnitObservable();
        }

        public static IObservable<Unit> OnAboutToShow(this Popup popup)
        {
            return popup.FromSignal("about_to_show").AsUnitObservable();
        }
    }
}
