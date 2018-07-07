using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;

namespace AlleyCat.Item
{
    public abstract class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        protected abstract IEquipmentHolder Holder { get; }

        protected override IDictionary<string, Equipment> CreateCache() =>
            Slots.Values
                .Select(s => s.GetParent(Holder).GetChildOrDefault<Equipment>())
                .Where(e => e != null)
                .Distinct()
                .ToDictionary(e => e.Slot);

        protected override void DoAdd(Equipment item)
        {
            item.GetParent()?.RemoveChild(item);

            var parent = Slots[item.Slot].GetParent(Holder);

            parent.AddChild(item);

            item.SetOwner(parent.GetOwner());
            item.Equip(Holder);
        }

        protected override void DoRemove(Equipment item)
        {
            item.Unequip(Holder);
            item.GetParent()?.RemoveChild(item);
        }

        public override bool AllowedFor(ISlotConfiguration context) =>
            (context is EquipmentConfiguration || context is Equipment) && base.AllowedFor(context);
    }
}
