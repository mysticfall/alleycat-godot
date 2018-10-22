using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public static class SceneTreeExtensions
    {
        public static IEnumerable<T> GetNodesInGroup<T>(this SceneTree tree, string group)
        {
            Ensure.That(tree, nameof(tree)).IsNotNull();
            Ensure.That(group, nameof(group)).IsNotNull();

            return tree.GetNodesInGroup(group).OfType<T>();
        }
    }
}
