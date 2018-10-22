using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public static class TreeExtensions
    {
        private const string NodeName = "TreeEventTracker";

        public static IObservable<TreeItemSelectedEvent> OnItemSelect(this Tree tree)
        {
            Ensure.That(tree, nameof(tree)).IsNotNull();

            return tree.GetComponent(NodeName, _ => new TreeEventTracker()).OnItemSelect;
        }
    }
}
