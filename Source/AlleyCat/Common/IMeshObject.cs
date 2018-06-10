using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IMeshObject : IBounded
    {
        IEnumerable<MeshInstance> Meshes { get; }
    }

    public static class MeshObjectExtensions
    {
        public static AABB CalculateBounds([NotNull] this IMeshObject source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            return source.Meshes.Select(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2));
        }
    }
}
