using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Item
{
    public abstract class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        protected abstract IEquipmentHolder Holder { get; }

        protected override IDictionary<string, Equipment> CreateCache() =>
            Slots.Values
                .SelectMany(s => s.GetParent(Holder).GetChildren<Equipment>())
                .Distinct()
                .ToDictionary(e => e.Slot);

        [PostConstruct(true)]
        protected virtual void OnInitialize()
        {
            foreach (var equipment in Values)
            {
                equipment.Equip(Holder);
            }
        }

        protected override void DoAdd(Equipment item)
        {
            var transform = item.GlobalTransform;

            item.GetParent()?.RemoveChild(item);

            var parent = Slots[item.Slot].GetParent(Holder);

            parent.AddChild(item);

            item.GlobalTransform = transform;

            item.SetOwner(parent.GetOwner());
            item.Equip(Holder);
        }

        protected override void DoRemove(Equipment item)
        {
            var transform = item.GlobalTransform;

            item.Unequip(Holder);
            item.GetParent()?.RemoveChild(item);

            item.GlobalTransform = transform;
        }

        public override bool AllowedFor(ISlotConfiguration context) =>
            (context is EquipmentConfiguration || context is Equipment) && base.AllowedFor(context);
    }
}
