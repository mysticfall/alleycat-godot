using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Physics
{
    public class Intersection : Collision
    {
        public Vector3 Position => (Vector3) Data["position"];

        public Vector3 Normal => (Vector3) Data["normal"];

        public Intersection([NotNull] IDictionary<object, object> data) : base(data)
        {
        }
    }
}
