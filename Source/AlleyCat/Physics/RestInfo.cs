using System.Collections.Generic;
using Godot;

namespace AlleyCat.Physics
{
    public class RestInfo : Collision
    {
        public Vector3 LinearVelocity => (Vector3) Data["linear_velocity"];

        public RestInfo(IDictionary<object, object> data) : base(data)
        {
        }
    }
}
