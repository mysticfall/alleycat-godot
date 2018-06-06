using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Physics
{
    public class Collision
    {
        public CollisionObject Collider => (CollisionObject) Data["collider"];

        public int ColliderId => (int) Data["collider_id"];

        public RID Rid => (RID) Data["rid"];

        public int Shape => (int) Data["shape"];

        public object Metadata => Data.TryGetValue("metadata", out var metadata) ? metadata : null;

        protected readonly IDictionary<object, object> Data;

        public Collision([NotNull] IDictionary<object, object> data)
        {
            Ensure.Any.IsNotNull(data, nameof(data));

            Data = data;
        }
    }
}
