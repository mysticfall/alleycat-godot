using System.Collections;
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

        public static Option<IIntersection> IntersectRay(
            this World world,
            Vector3 from,
            Vector3 to,
            int collisionLayer = NoCollisionLayer)
        {
            return IntersectRay(world, from, to, None, collisionLayer);
        }

        public static Option<IIntersection> IntersectRay(
            this World world,
            Vector3 from,
            Vector3 to,
            Array exclude,
            int collisionLayer = NoCollisionLayer)
        {
            return IntersectRay(world, from, to, Some(exclude), collisionLayer);
        }

        private static Option<IIntersection> IntersectRay(
            this World world,
            Vector3 from,
            Vector3 to,
            Option<Array> exclude,
            int collisionLayer = NoCollisionLayer)
        {
            Ensure.That(world, nameof(world)).IsNotNull();

            var state = world.DirectSpaceState;
            var result = state.IntersectRay(from, to, exclude.ValueUnsafe(), collisionLayer);

            return result.Contains("collider") ? Some((IIntersection) new Intersection(result)) : None;
        }

        public static IEnumerable<ICollision> IntersectShape(
            this World world,
            PhysicsShapeQueryParameters shape,
            int maxResults = 32)
        {
            Ensure.That(world, nameof(world)).IsNotNull();

            return world.DirectSpaceState
                .IntersectShape(shape, maxResults)
                .Cast<IDictionary>()
                .Filter(d => d.Contains("collider"))
                .Map(d => (ICollision) new Collision(d));
        }

        public static IEnumerable<ICollision> CollideShape(
            this World world,
            PhysicsShapeQueryParameters shape,
            int maxResults = 32)
        {
            Ensure.That(world, nameof(world)).IsNotNull();

            return world.DirectSpaceState
                .CollideShape(shape, maxResults)
                .Cast<IDictionary>()
                .Filter(d => d.Contains("collider"))
                .Map(d => (ICollision) new Collision(d));
        }

        public static Option<IRestInfo> GetRestInfo(
            this World world,
            PhysicsShapeQueryParameters shape)
        {
            Ensure.That(world, nameof(world)).IsNotNull();

            var state = world.DirectSpaceState;
            var result = state.GetRestInfo(shape);

            return result.Contains("collider") ? Some((IRestInfo) new RestInfo(result)) : None;
        }
    }
}
