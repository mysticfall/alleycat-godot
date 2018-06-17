using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public abstract class Equipment : SlotItem
    {
        [Export, UsedImplicitly] private PackedScene _equippableItem;

        public EquippableItem CreateItem() => (EquippableItem) _equippableItem?.Instance();

        public abstract void Equip(IEquipmentHolder holder);

        public abstract void Unequip();
    }
}
