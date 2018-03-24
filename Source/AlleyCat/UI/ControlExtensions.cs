using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public static class ControlExtensions
    {
        private const string NodeName = "ControlEventTracker";

        [NotNull]
        public static IObservable<MouseEnteredEvent> OnMouseEnter(
            [NotNull] this Godot.Control control)
        {
            Ensure.Any.IsNotNull(control, nameof(control));

            var tracker = control.GetOrCreateNode(NodeName, _ => new ControlEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnMouseEnter;
        }

        [NotNull]
        public static IObservable<MouseExitedEvent> OnMouseExit(
            [NotNull] this Godot.Control control)
        {
            Ensure.Any.IsNotNull(control, nameof(control));

            var tracker = control.GetOrCreateNode(NodeName, _ => new ControlEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnMouseExit;
        }
    }
}
