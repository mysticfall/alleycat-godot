using System.Collections.Generic;

namespace AlleyCat.Character.Morph
{
    public interface IMorphableRace : IRace
    {
        IEnumerable<IMorphGroup> GetMorphGroups(Sex sex);
    }
}
