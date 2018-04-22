using System.Collections.Generic;

namespace AlleyCat.Item
{
    public interface IEquipmentHolder
    {
        IEnumerable<EquipmentSlot> EquipmentSlots { get; }

        IEquipmentContainer Equipments { get; }
    }
}
