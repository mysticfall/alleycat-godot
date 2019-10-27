using System;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public static class TreeExtensions
    {
        public static void RemoveAllNodes(this Tree tree)
        {
            Ensure.That(tree, nameof(tree)).IsNotNull();

            tree.GetRoot().Children().Iter(c => c.Free());
        }

        public static IObservable<Option<TreeItem>> OnItemSelect(this Tree tree)
        {
            return tree.FromSignal("item_selected").Select(_ => Optional(tree.GetSelected()));
        }
    }
}
