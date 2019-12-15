using System.Collections.Generic;
using System.Linq;
using AlleyCat.Morph;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    public class MorphableRaceFactory : BaseRaceFactory<MorphableRace>
    {
        [Export]
        public IEnumerable<SexMorphMapping> MorphMappings { get; set; }

        protected override Validation<string, MorphableRace> CreateResource(string key, string displayName)
        {
            var slots = Optional(EquipmentSlots)
                .Flatten()
                .Map(s => s.Service)
                .Sequence();

            var groups = Optional(MorphMappings)
                .Flatten()
                .Map(m => (m.Sex, Groups: Optional(m.MorphGroups).Flatten().Map(f => f.Service).Sequence()))
                .Map(t => t.Groups.Map(g => (t.Sex, Groups: g.OfType<IMorphGroup>())))
                .Sequence()
                .Map(toMap);

            return
                from s in slots
                from g in groups
                select new MorphableRace(key, displayName, s, g);
        }
    }
}
