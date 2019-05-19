using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public static class LineEditExtensions
    {
        public static IObservable<string> OnTextChanged(this LineEdit edit)
        {
            return edit.FromSignal("text_changed")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable());
        }

        public static IObservable<string> OnTextEntered(this LineEdit edit)
        {
            return edit.FromSignal("text_entered")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable());
        }
    }
}
