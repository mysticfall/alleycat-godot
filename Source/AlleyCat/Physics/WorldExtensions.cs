using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using Godot.Collections;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace AlleyCat.Physics
{
    public static class WorldExtensions
    {
        public const int NoCollisionLayer = 2147483647;

        public static Option<Intersection> IntersectRay(
            this World world,
            Vector3 from,
            Vector3 to,
            int collisionLayer = NoCollisionLayer)
        {
            return IntersectRay(world, from, to, None, collisionLayer);
        }

        public static Option<Intersection> IntersectRay(
            this World world,
            Vector3 from,
            Vector3 to,
            Array exclude,
            int collisionLayer = NoCollisionLayer)
        {
            return IntersectRay(world, from, to, Some(exclude), collisionLayer);
        }

        private static Option<Intersection> IntersectRay(
            this World world,
            Vector3 from,
            Vector3 to,
            Option<Array> exclude,
            int collisionLayer = NoCollisionLayer)
        {
            Ensure.That(world, nameof(world)).IsNotNull();

            var state = world.DirectSpaceState;
            var result = state.IntersectRay(from, to, exclude.ValueUnsafe(), collisionLayer);

            return result.ContainsKey("collider") ? Some(new Intersection(result)) : None;
        }

        public static IEnumerable<Collision> IntersectShape(
            this World world,
            PhysicsShapeQueryParameters shape,
            int maxResults = 32)
        {
            Ensure.That(world, nameof(world)).IsNotNull();
            Ensure.That(shape, nameof(shape)).IsNotNull();
            Ensure.That(maxResults, nameof(maxResults)).IsGt(0);

            return world.DirectSpaceState
                .IntersectShape(shape, maxResults)
                .Cast<IDictionary<object, object>>()
                .Where(d => d.ContainsKey("collider"))
                .Select(d => new Collision(d));
        }

        public static IEnumerable<Collision> CollideShape(
            this World world,
            PhysicsShapeQueryParameters shape,
            int maxResults = 32)
        {
            Ensure.That(world, nameof(world)).IsNotNull();
            Ensure.That(shape, nameof(shape)).IsNotNull();
            Ensure.That(maxResults, nameof(maxResults)).IsGt(0);

            return world.DirectSpaceState
                .CollideShape(shape, maxResults)
                .Cast<IDictionary<object, object>>()
                .Where(d => d.ContainsKey("collider"))
                .Select(d => new Collision(d));
        }

        public static Option<RestInfo> GetRestInfo(
            this World world,
            PhysicsShapeQueryParameters shape)
        {
            Ensure.That(world, nameof(world)).IsNotNull();
            Ensure.That(shape, nameof(shape)).IsNotNull();

            var state = world.DirectSpaceState;
            var result = state.GetRestInfo(shape);

            return result.ContainsKey("collider") ? Some(new RestInfo(result)) : None;
        }
    }
}
