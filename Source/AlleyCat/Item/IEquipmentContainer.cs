using AlleyCat.Item.Generic;

namespace AlleyCat.Item
{
    public interface IEquipmentContainer : ISlotContainer<EquipmentSlot, IEquippable>
    {
        IEquipmentHolder Holder { get; }
    }
}
