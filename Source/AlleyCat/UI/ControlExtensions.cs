using System;
using AlleyCat.Common;
using EnsureThat;

namespace AlleyCat.UI
{
    public static class ControlExtensions
    {
        private const string NodeName = "ControlEventTracker";

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
