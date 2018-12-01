using System.Collections.Generic;
using EnsureThat;

namespace AlleyCat.Physics
{
    public struct RestInfo : IRestInfo
    {
        public IDictionary<object, object> RawData { get; }

        public RestInfo(IDictionary<object, object> data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            RawData = data;
        }
    }
}
