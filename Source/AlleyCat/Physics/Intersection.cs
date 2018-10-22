using System.Collections.Generic;
using Godot;

namespace AlleyCat.Physics
{
    public class Intersection : Collision
    {
        public Vector3 Position => (Vector3) Data["position"];

        public Vector3 Normal => (Vector3) Data["normal"];

        public Intersection(IDictionary<object, object> data) : base(data)
        {
        }
    }
}
