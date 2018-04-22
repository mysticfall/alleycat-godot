using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface IEquippable : ISlotItem, IInventoryItem
    {
        void OnEquipped([NotNull] IEquipmentContainer container);

        void OnUnequipped([NotNull] IEquipmentContainer container);
    }
}
