using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public static class TreeExtensions
    {
        private const string NodeName = "TreeEventTracker";

        [NotNull]
        public static IObservable<TreeItemSelectedEvent> OnItemSelect([NotNull] this Tree tree)
        {
            Ensure.Any.IsNotNull(tree, nameof(tree));

            var tracker = tree.GetOrCreateNode(NodeName, _ => new TreeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnItemSelect;
        }
    }
}
