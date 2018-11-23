using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        public IEquipmentHolder Holder { get; }

        public override Map<string, EquipmentSlot> Slots { get; }

        protected override IEnumerable<Equipment> InitialItems =>
            Slots.Values
                .Map(s => s.GetParent(Holder))
                .Distinct()
                .Bind(p => p.GetChildComponents<Equipment>());

        public EquipmentContainer(
            IEnumerable<EquipmentSlot> slots,
            IEquipmentHolder holder,
            ILogger logger) : base(logger)
        {
            Ensure.That(slots, nameof(slots)).IsNotNull();
            Ensure.That(holder, nameof(holder)).IsNotNull();

            Holder = holder;
            Slots = slots.ToMap();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Items.Values.Iter(v => v.Equip(Holder));
        }

        protected override void DoAdd(Equipment item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var transform = item.GetGlobalTransform();

            Optional(item.Node.GetParent()).Iter(p => p.RemoveChild(item.Node));

            var parent = Slots[item.Slot].GetParent(Holder);

            parent.AddChild(item.Node);

            item.SetGlobalTransform(transform);

            item.Node.SetOwner(parent.GetOwner());
            item.Equip(Holder);
        }

        protected override void DoRemove(Equipment item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var transform = item.GetGlobalTransform();

            item.Unequip(Holder);

            Optional(item.Node.GetParent()).Iter(p => p.RemoveChild(item.Node));

            item.SetGlobalTransform(transform);
        }

        public override bool AllowedFor(ISlotConfiguration context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return (context is EquipmentConfiguration || context is Equipment) && base.AllowedFor(context);
        }

        protected override void PreDestroy()
        {
            Items.Values.Iter(v => v.DisposeQuietly());

            base.PreDestroy();
        }
    }
}
