using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Common
{
    public static class SpatialExtensions
    {
        public static IObservable<bool> OnVisibilityChange(this Spatial node)
        {
            return node.FromSignal("visibility_changed").Select(_ => node.Visible);
        }
    }
}
