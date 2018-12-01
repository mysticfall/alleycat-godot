using EnsureThat;
using Godot;

namespace AlleyCat.Physics
{
    public interface IRestInfo : ICollision
    {
    }

    public static class RestInfoExtensions
    {
        public static Vector3 GetLinearVelocity(this IRestInfo info)
        {
            Ensure.That(info, nameof(info)).IsNotNull();

            return (Vector3) info.RawData["linear_velocity"];
        }
    }
}
