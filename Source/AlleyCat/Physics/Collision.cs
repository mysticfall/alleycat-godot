using System.Collections.Generic;
using EnsureThat;

namespace AlleyCat.Physics
{
    public struct Collision : ICollision
    {
        public IDictionary<object, object> RawData { get; }

        public Collision(IDictionary<object, object> data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            RawData = data;
        }
    }
}
