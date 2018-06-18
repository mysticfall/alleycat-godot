using AlleyCat.Animation;

namespace AlleyCat.Item
{
    public abstract class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        protected abstract IEquipmentHolder Holder { get; }

        public override void Add(Equipment item)
        {
            base.Add(item);

            item.Equip(Holder);

            var animator = Holder.AnimationManager as IAnimationStateManager;
            var animation = item.Animation;

            if (animator != null && animation != null)
            {
                //TODO Should leave some 'room' for root motion animations so that they can still fire Reset().
                animator.Blend(animation, 0.99f);
            }
        }

        public override void Remove(Equipment item)
        {
            var animator = Holder.AnimationManager as IAnimationStateManager;
            var animation = item.Animation;

            if (animator != null && animation != null)
            {
                animator.Unblend(animation.GetName());
            }

            item.Unequip();

            base.Remove(item);
        }

        public override bool AllowedFor(ISlotConfiguration context) =>
            (context is EquipConfiguration || context is Equipment) && base.AllowedFor(context);
    }
}
