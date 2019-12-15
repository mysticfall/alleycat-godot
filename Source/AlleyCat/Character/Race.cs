using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Item;
using EnsureThat;

namespace AlleyCat.Character
{
    public class Race : IGameResource, INamed
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public IEnumerable<EquipmentSlot> EquipmentSlots { get; }

        public Race(
            string key,
            string displayName,
            IEnumerable<EquipmentSlot> equipmentSlots)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(equipmentSlots, nameof(equipmentSlots)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            EquipmentSlots = equipmentSlots.Freeze();
        }
    }
}
