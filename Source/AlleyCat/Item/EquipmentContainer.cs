using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        protected abstract IEquipmentHolder Holder { get; }

        protected override Map<string, Equipment> CreateCache() =>
            toMap(Slots.Values
                .Bind(s => s.GetParent(Holder).GetChildComponents<Equipment>())
                .Distinct()
                .Map(v => (v.Slot, v)));

        [PostConstruct(true)]
        protected virtual void OnInitialize()
        {
            Values.Iter(v => v.Equip(Holder));
        }

        protected override void DoAdd(Equipment item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var transform = item.GlobalTransform;

            Optional(item.GetParent()).Iter(p => p.RemoveChild(item));

            var parent = Slots[item.Slot].GetParent(Holder);

            parent.AddChild(item);

            item.GlobalTransform = transform;

            item.SetOwner(parent.GetOwner());
            item.Equip(Holder);
        }

        protected override void DoRemove(Equipment item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var transform = item.GlobalTransform;

            item.Unequip(Holder);

            Optional(item.GetParent()).Iter(p => p.RemoveChild(item));

            item.GlobalTransform = transform;
        }

        public override bool AllowedFor(ISlotConfiguration context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return (context is EquipmentConfiguration || context is Equipment) && base.AllowedFor(context);
        }
    }
}
