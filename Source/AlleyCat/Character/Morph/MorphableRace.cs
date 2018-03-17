using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;

namespace AlleyCat.Character.Morph
{
    public class MorphableRace : Race, IMorphableRace
    {
        protected virtual string MorphGroupPath { get; } = "Morphs";

        protected virtual string GetMorphGroupPath(Sex sex) => MorphGroupPath + "/" + sex;

        public IEnumerable<IMorphGroup> GetMorphGroups(Sex sex) =>
            GetNode(GetMorphGroupPath(sex))?.GetChildren<IMorphGroup>() ?? Enumerable.Empty<IMorphGroup>();
    }
}
