using System.Diagnostics;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IBounded : ITransformable
    {
        AABB Bounds { get; }
    }

    public static class BoundedExtensions
    {
        public static Vector3 Center(this IBounded bounded)
        {
            Ensure.That(bounded, nameof(bounded)).IsNotNull();

            Debug.Assert(bounded.Spatial != null, "bounded.Spatial != null");

            var bounds = bounded.Bounds;

            return bounded.Spatial.GlobalTransform.origin + (bounds.Position + bounds.End) / 2f;
        }
    }
}
