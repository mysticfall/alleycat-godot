using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public static class ControlExtensions
    {
        public static Option<Color> FindColor(this Godot.Control control, string name, string type)
        {
            Ensure.That(control, nameof(control)).IsNotNull();

            return control.HasColor(name, type) ? Some(control.GetColor(name, type)) : None;
        }

        public static Option<Font> FindFont(this Godot.Control control, string name, string type)
        {
            Ensure.That(control, nameof(control)).IsNotNull();

            return control.HasFont(name, type) ? Some(control.GetFont(name, type)) : None;
        }

        public static IObservable<bool> OnVisibilityChange(this Godot.Control control)
        {
            return control.FromSignal("visibility_changed").Select(_ => control.Visible);
        }

        public static IObservable<Unit> OnMouseEnter(this Godot.Control control)
        {
            return control.FromSignal("mouse_entered").Select(_ => Unit.Default);
        }

        public static IObservable<Unit> OnMouseExit(this Godot.Control control)
        {
            return control.FromSignal("mouse_exited").Select(_ => Unit.Default);
        }
    }
}
