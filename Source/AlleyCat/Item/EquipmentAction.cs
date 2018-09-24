using AlleyCat.Action;

namespace AlleyCat.Item
{
    public abstract class EquipmentAction : Interaction
    {
        protected override void DoExecute(InteractionContext context)
        {
            if (context.Actor is IEquipmentHolder holder &&
                context.Target is Equipment equipment)
            {
                DoExecute(holder, equipment, context);
            }
        }

        protected abstract void DoExecute(
            IEquipmentHolder holder,
            Equipment equipment,
            InteractionContext context);

        protected override bool AllowedFor(InteractionContext context) =>
            context.Actor is IEquipmentHolder holder &&
            context.Target is Equipment equipment &&
            AllowedFor(holder, equipment, context);

        protected abstract bool AllowedFor(
            IEquipmentHolder holder,
            Equipment equipment,
            InteractionContext context);
    }
}
