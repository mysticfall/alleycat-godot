using System.Collections.Generic;
using EnsureThat;

namespace AlleyCat.Physics
{
    public struct Intersection : IIntersection
    {
        public IDictionary<object, object> RawData { get; }

        public Intersection(IDictionary<object, object> data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            RawData = data;
        }
    }
}
