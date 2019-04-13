using System.Collections.Generic;
using AlleyCat.Common;

namespace AlleyCat.Morph
{
    public interface IMorphGroup : INamed
    {
        IEnumerable<IMorphDefinition> Definitions { get; }
    }
}
