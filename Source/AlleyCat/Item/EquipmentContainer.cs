using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Item
{
    [Singleton(typeof(IEquipmentContainer))]
    public class EquipmentContainer : SlotContainer<EquipmentSlot, IEquippable>, IEquipmentContainer
    {
        [Ancestor]
        public IEquipmentHolder Holder { get; private set; }

        public override IReadOnlyDictionary<string, EquipmentSlot> Slots =>
            Holder.EquipmentSlots.ToDictionary();

        [PostConstruct(true)]
        protected virtual void OnInitialize()
        {
            Values.ToList().ForEach(i => i.OnEquipped(this));
        }

        public override IEquippable Add(IEquippable item)
        {
            var replacing = base.Add(item);

            replacing?.OnUnequipped(this);
            item.OnEquipped(this);

            return replacing;
        }

        public override void Remove(IEquippable item)
        {
            base.Remove(item);

            item.OnUnequipped(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Values.ToList().ForEach(i => i.OnUnequipped(this));
            }

            base.Dispose(disposing);
        }
    }
}
