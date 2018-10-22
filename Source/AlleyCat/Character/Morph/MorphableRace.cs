using System.Collections.Generic;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Character.Morph
{
    public class MorphableRace : Race, IMorphableRace
    {
        protected virtual string GetMorphGroupPath(Sex sex) => "Morphs/" + sex;

        public IEnumerable<IMorphGroup> GetMorphGroups(Sex sex)
        {
            var path = GetMorphGroupPath(sex);

            return this.FindComponent<Node>(path).Bind(c => c.GetChildComponents<IMorphGroup>());
        }
    }
}
