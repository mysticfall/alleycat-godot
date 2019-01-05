using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Common
{
    public static class ViewportExtensions
    {
        public static IObservable<Vector2> OnSizeChange(this Viewport viewport)
        {
            return viewport.FromSignal("size_changed").Select(_ => viewport.Size);
        }
    }
}
