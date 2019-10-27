using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public static class AcceptDialogExtensions
    {
        public static IObservable<Unit> OnConfirm(this AcceptDialog dialog)
        {
            return dialog.FromSignal("confirmed").AsUnitObservable();
        }

        public static IObservable<string> OnCustomAction(this AcceptDialog dialog)
        {
            return dialog.FromSignal("custom_action")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable());
        }
    }
}
