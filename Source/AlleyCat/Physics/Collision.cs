using System.Collections;
using EnsureThat;

namespace AlleyCat.Physics
{
    public struct Collision : ICollision
    {
        public IDictionary RawData { get; }

        public Collision(IDictionary data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            RawData = data;
        }
    }
}
