using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public interface IMorphableRace : IRace
    {
        [NotNull]
        IEnumerable<IMorphGroup> GetMorphGroups(Sex sex);
    }
}
