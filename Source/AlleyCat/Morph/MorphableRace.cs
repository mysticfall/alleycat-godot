using System.Collections.Generic;
using AlleyCat.Item;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public class MorphableRace : Race
    {
        public Map<Sex, IEnumerable<IMorphGroup>> MorphGroups { get; }

        public MorphableRace(
            string key,
            string displayName,
            IEnumerable<EquipmentSlot> equipmentSlots,
            Map<Sex, IEnumerable<IMorphGroup>> morphGroups,
            ILoggerFactory loggerFactory) : base(key, displayName, equipmentSlots, loggerFactory)
        {
            MorphGroups = morphGroups;
        }
    }
}
