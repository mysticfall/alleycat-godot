using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public static class ControlExtensions
    {
        private const string NodeName = "ControlEventTracker";

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

        public static IObservable<MouseEnteredEvent> OnMouseEnter(this Godot.Control control)
        {
            Ensure.That(control, nameof(control)).IsNotNull();

            return control.GetComponent(NodeName, _ => new ControlEventTracker()).OnMouseEnter;
        }

        public static IObservable<MouseExitedEvent> OnMouseExit(this Godot.Control control)
        {
            Ensure.That(control, nameof(control)).IsNotNull();

            return control.GetComponent(NodeName, _ => new ControlEventTracker()).OnMouseExit;
        }
    }
}
