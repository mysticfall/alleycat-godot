using EnsureThat;
using Godot;

namespace AlleyCat.Physics
{
    public interface IIntersection : ICollision
    {
    }

    public static class IntersectionExtensions
    {
        public static Vector3 GetPosition(this IIntersection intersection)
        {
            Ensure.That(intersection, nameof(intersection)).IsNotNull();

            return (Vector3) intersection.RawData["position"];
        }

        public static Vector3 GetNormal(this IIntersection intersection)
        {
            Ensure.That(intersection, nameof(intersection)).IsNotNull();

            return (Vector3) intersection.RawData["normal"];
        }
    }
}
