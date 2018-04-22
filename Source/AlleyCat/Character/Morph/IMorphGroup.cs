using System.Collections.Generic;
using AlleyCat.Common;

namespace AlleyCat.Character.Morph
{
    public interface IMorphGroup : INamed, IReadOnlyDictionary<string, IMorphDefinition>
    {
    }
}
