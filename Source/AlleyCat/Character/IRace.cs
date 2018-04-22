using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Item;

namespace AlleyCat.Character
{
    public interface IRace : INamed
    {
        IEnumerable<EquipmentSlot> EquipmentSlots { get; }
    }
}
