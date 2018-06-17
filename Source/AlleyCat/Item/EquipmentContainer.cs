namespace AlleyCat.Item
{
    public abstract class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        protected abstract IEquipmentHolder Holder { get; }

        public override void Add(Equipment item)
        {
            base.Add(item);

            item.Equip(Holder);
        }

        public override void Remove(Equipment item)
        {
            item.Unequip();

            base.Remove(item);
        }

        public override bool AllowedFor(ISlotConfiguration context) =>
            (context is EquipConfiguration || context is Equipment) && base.AllowedFor(context);
    }
}
