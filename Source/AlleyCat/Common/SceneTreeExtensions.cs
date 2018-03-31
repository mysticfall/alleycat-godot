using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class SceneTreeExtensions
    {
        [NotNull]
        public static IEnumerable<T> GetNodesInGroup<T>([NotNull] this SceneTree sceneTree, [NotNull] string group)
        {
            Ensure.Any.IsNotNull(sceneTree, nameof(sceneTree));
            Ensure.Any.IsNotNull(group, nameof(group));

            return sceneTree.GetNodesInGroup(group).OfType<T>();
        }
    }
}
