using System.Linq;
using AlleyCat.Action;
using AlleyCat.Game;
using EnsureThat;
using JetBrains.Annotations;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class DropAction : EquipmentAction
    {
        protected override void DoExecute(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            var scene = GetTree().CurrentScene;
            var path = ((IScene) scene).ItemsPath;

            var parent = scene.GetNode(path) ?? scene;

            holder.Unequip(equipment, parent);
        }

        protected override bool AllowedFor(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            return equipment.Slot != null && 
                   holder.HasEquipment(equipment.Slot) &&
                   equipment.Configuration.HasTag(Carry);
        }
    }

    public static class DropActionExtensions
    {
        public static void Pickup([NotNull] this IEquipmentHolder holder, [NotNull] Equipment equipment)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));
            Ensure.Any.IsNotNull(equipment, nameof(equipment));

            if (!(holder is IActor actor)) return;

            var action = actor.Actions.Values.FirstOrDefault(a => a is DropAction);
            var context = new InteractionContext(actor, equipment);

            if (action?.AllowedFor(context) ?? false)
            {
                action.Execute(context);
            }
        }
    }
}
