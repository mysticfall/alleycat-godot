using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public static class RangeExtensions
    {
        public static IObservable<float> OnValueChange(this Range range)
        {
            return range.FromSignal("value_changed")
                .SelectMany(args => args.HeadOrNone().OfType<float>().ToObservable());
        }
    }
}
