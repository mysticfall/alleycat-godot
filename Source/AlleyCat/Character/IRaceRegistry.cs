using System.Collections.Generic;

namespace AlleyCat.Character
{
    public interface IRaceRegistry : IReadOnlyDictionary<string, IRace>
    {
    }
}
