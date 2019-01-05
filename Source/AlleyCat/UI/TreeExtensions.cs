using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public static class TreeExtensions
    {
        public static IObservable<Option<TreeItem>> OnItemSelect(this Tree tree)
        {
            return tree.FromSignal("item_selected").Select(_ => Optional(tree.GetSelected()));
        }
    }
}
