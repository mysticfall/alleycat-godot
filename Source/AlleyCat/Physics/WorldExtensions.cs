using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;

namespace AlleyCat.Physics
{
    public static class WorldExtensions
    {
        public const int NoCollisionLayer = 2147483647;

        [CanBeNull]
        public static Intersection IntersectRay(
            [NotNull] this World world,
            Vector3 from,
            Vector3 to,
            Array exclude = null,
            int collisionLayer = NoCollisionLayer)
        {
            Ensure.Any.IsNotNull(world, nameof(world));

            var state = world.DirectSpaceState;
            var result = state.IntersectRay(from, to, exclude, collisionLayer);

            return result.ContainsKey("collider") ? new Intersection(result) : null;
        }

        [NotNull]
        public static IEnumerable<Collision> IntersectShape(
            [NotNull] this World world,
            [NotNull] PhysicsShapeQueryParameters shape,
            int maxResults = 32)
        {
            Ensure.Any.IsNotNull(world, nameof(world));
            Ensure.Any.IsNotNull(shape, nameof(shape));

            return world.DirectSpaceState
                .IntersectShape(shape, maxResults)
                .Cast<IDictionary<object, object>>()
                .Where(d => d.ContainsKey("collider"))
                .Select(d => new Collision(d));
        }

        [NotNull]
        public static IEnumerable<Collision> CollideShape(
            [NotNull] this World world,
            [NotNull] PhysicsShapeQueryParameters shape,
            int maxResults = 32)
        {
            Ensure.Any.IsNotNull(world, nameof(world));
            Ensure.Any.IsNotNull(shape, nameof(shape));

            return world.DirectSpaceState
                .CollideShape(shape, maxResults)
                .Cast<IDictionary<object, object>>()
                .Where(d => d.ContainsKey("collider"))
                .Select(d => new Collision(d));
        }

        [CanBeNull]
        public static RestInfo GetRestInfo(
            [NotNull] this World world,
            [NotNull] PhysicsShapeQueryParameters shape)
        {
            Ensure.Any.IsNotNull(world, nameof(world));
            Ensure.Any.IsNotNull(shape, nameof(shape));
            
            var state = world.DirectSpaceState;
            var result = state.GetRestInfo(shape);

            return result.ContainsKey("collider") ? new RestInfo(result) : null;
        }
    }
}
