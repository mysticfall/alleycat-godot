using System.Diagnostics;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface ITransformable
    {
        Spatial Spatial { get; }
    }

    public static class TransformableExtensions
    {
        public static Transform GetTransform(this ITransformable transformable)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            return transformable.Spatial.Transform;
        }

        public static void SetTransform(this ITransformable transformable, Transform transform)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            transformable.Spatial.Transform = transform;
        }

        public static Transform GetGlobalTransform(this ITransformable transformable)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            return transformable.Spatial.GlobalTransform;
        }

        public static void SetGlobalTransform(this ITransformable transformable, Transform transform)
        {
            Ensure.That(transformable, nameof(transformable)).IsNotNull();

            Debug.Assert(transformable.Spatial != null, "transformable.Spatial != null");

            transformable.Spatial.GlobalTransform = transform;
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
