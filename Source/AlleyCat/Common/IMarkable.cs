using System.Collections.Generic;

namespace AlleyCat.Common
{
    public interface IMarkable
    {
        IReadOnlyDictionary<string, Marker> Markers { get; }
    }
}
