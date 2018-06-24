using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IBounded : ITransformable
    {
        AABB Bounds { get; }
    }

    public static class BoundedExtensions
    {
        public static Vector3 Center([NotNull] this IBounded bounded)
        {
            Ensure.Any.IsNotNull(bounded, nameof(bounded));

            var bounds = bounded.Bounds;

            return bounded.Spatial.GlobalTransform.origin + (bounds.Position + bounds.End) / 2f;
        }
    }
}
