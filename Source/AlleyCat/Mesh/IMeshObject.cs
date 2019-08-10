using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    [NonInjectable]
    public interface IMeshObject : IBounded, IHideable
    {
        IEnumerable<MeshInstance> Meshes { get; }
    }

    public static class MeshObjectExtensions
    {
        public static AABB CalculateBounds(this IMeshObject source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Debug.Assert(source.Meshes != null, "source.Meshes != null");

            return source.Meshes.Any()
                ? source.Meshes.Map(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2))
                : new AABB(source.Origin(), Vector3.Zero);
        }
    }
}
