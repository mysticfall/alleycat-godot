using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface ITransformable
    {
        Spatial Spatial { get; }
    }

    public static class TransformableExtentions {

        public static Transform Transform([NotNull] this ITransformable transformable)
        {
            Ensure.Any.IsNotNull(transformable, nameof(transformable));

            return transformable.Spatial.Transform;
        }

        public static Transform GlobalTransform([NotNull] this ITransformable transformable)
        {
            Ensure.Any.IsNotNull(transformable, nameof(transformable));

            return transformable.Spatial.GlobalTransform;
        }
    }
}
