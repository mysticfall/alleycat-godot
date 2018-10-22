using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Physics
{
    public class Collision
    {
        public CollisionObject Collider => (CollisionObject) Data["collider"];

        public int ColliderId => (int) Data["collider_id"];

        public RID Rid => (RID) Data["rid"];

        public int Shape => (int) Data["shape"];

        public Option<object> Metadata => Data.TryGetValue("metadata");

        protected readonly IDictionary<object, object> Data;

        public Collision(IDictionary<object, object> data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            Data = data;
        }
    }
}
