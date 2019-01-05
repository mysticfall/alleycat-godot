using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public static class TreeExtensions
    {
        public static IObservable<TreeItemSelectedEvent> OnItemSelect(this Tree tree)
        {
            return tree.FromSignal("item_selected").Select(_ => new TreeItemSelectedEvent(tree));
        }
    }
}
