using System.Collections;
using EnsureThat;

namespace AlleyCat.Physics
{
    public struct Intersection : IIntersection
    {
        public IDictionary RawData { get; }

        public Intersection(IDictionary data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            RawData = data;
        }
    }
}
