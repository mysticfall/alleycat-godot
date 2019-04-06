using System.Collections;
using EnsureThat;

namespace AlleyCat.Physics
{
    public struct RestInfo : IRestInfo
    {
        public IDictionary RawData { get; }

        public RestInfo(IDictionary data)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            RawData = data;
        }
    }
}
