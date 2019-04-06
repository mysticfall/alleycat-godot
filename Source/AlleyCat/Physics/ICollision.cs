using System.Collections;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Physics
{
    public interface ICollision
    {
        IDictionary RawData { get; }
    }

    public static class CollisionInfoExtensions
    {
        public static Spatial GetCollider(this ICollision collision)
        {
            Ensure.That(collision, nameof(collision)).IsNotNull();

            return (Spatial) collision.RawData["collider"];
        }

        public static int GetColliderId(this ICollision collision)
        {
            Ensure.That(collision, nameof(collision)).IsNotNull();

            return (int) collision.RawData["collider_id"];
        }

        public static RID GetRID(this ICollision collision)
        {
            Ensure.That(collision, nameof(collision)).IsNotNull();

            return (RID) collision.RawData["rid"];
        }

        public static int GetShape(this ICollision collision)
        {
            Ensure.That(collision, nameof(collision)).IsNotNull();

            return (int) collision.RawData["shape"];
        }

        public static Option<object> GetMetadata(this ICollision collision)
        {
            Ensure.That(collision, nameof(collision)).IsNotNull();

            return collision.RawData["metadata"];
        }
    }
}
