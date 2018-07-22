using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface ITransformable
    {
        Spatial Spatial { get; }
    }

    public static class TransformableExtensions
    {
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

        public static Vector3 Origin([NotNull] this ITransformable transformable)
        {
            Ensure.Any.IsNotNull(transformable, nameof(transformable));

            return transformable.Spatial.GlobalTransform.origin;
        }

        public static float DistanceTo([NotNull] this ITransformable transformable, [NotNull] ITransformable target)
        {
            Ensure.Any.IsNotNull(transformable, nameof(transformable));
            Ensure.Any.IsNotNull(target, nameof(target));

            return Origin(transformable).DistanceTo(Origin(target));
        }
    }
}
