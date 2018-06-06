using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Physics
{
    public class RestInfo : Collision
    {
        public Vector3 LinearVelocity => (Vector3) Data["linear_velocity"];

        public RestInfo([NotNull] IDictionary<object, object> data) : base(data)
        {
        }
    }
}
