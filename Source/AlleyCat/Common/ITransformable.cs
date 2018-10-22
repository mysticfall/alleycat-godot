using System.Diagnostics;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public interface ITransformable
    {
        Spatial Spatial { get; }
    }

    public static class TransformableExtensions
    {
        public static Transform Transform(this ITransformable transformable)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            return transformable.Spatial.Transform;
        }

        public static Transform GlobalTransform(this ITransformable transformable)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            return transformable.Spatial.GlobalTransform;
        }

        public static Vector3 Origin(this ITransformable transformable)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            return transformable.Spatial.GlobalTransform.origin;
        }

        public static float DistanceTo(this ITransformable transformable, ITransformable target)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();
            Ensure.That(target, nameof(target)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");
            Debug.Assert(target.Spatial != null, "target.Spatial != null");

            return Origin(transformable).DistanceTo(Origin(target));
        }
    }
}
