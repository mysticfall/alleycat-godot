using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public static class RangeExtensions
    {
        private const string NodeName = "RangeEventTracker";

        public static IObservable<ValueChangedEvent> OnValueChange(this Range range)
        {
            Ensure.That(range, nameof(range)).IsNotNull();

            return range.GetComponent(NodeName, _ => new RangeEventTracker()).OnValueChange;
        }
    }
}
