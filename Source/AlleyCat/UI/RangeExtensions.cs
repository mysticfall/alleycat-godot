using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public static class RangeExtensions
    {
        private const string NodeName = "RangeEventTracker";

        [NotNull]
        public static IObservable<ValueChangedEvent> OnValueChange(
            [NotNull] this Range range)
        {
            Ensure.Any.IsNotNull(range, nameof(range));

            var tracker = range.GetOrCreateNode(NodeName, _ => new RangeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnValueChange;
        }
    }
}
