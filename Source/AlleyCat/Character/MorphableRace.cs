using System.Collections.Generic;
using AlleyCat.Item;
using AlleyCat.Morph;
using LanguageExt;

namespace AlleyCat.Character
{
    public class MorphableRace : Race
    {
        public Map<Sex, IEnumerable<IMorphGroup>> MorphGroups { get; }

        public MorphableRace(
            string key,
            string displayName,
            IEnumerable<EquipmentSlot> equipmentSlots,
            Map<Sex, IEnumerable<IMorphGroup>> morphGroups) : base(key, displayName, equipmentSlots)
        {
            MorphGroups = morphGroups;
        }
    }
}
