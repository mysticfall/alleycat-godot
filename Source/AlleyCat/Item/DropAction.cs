using System;
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
        public static void Drop<T>([NotNull] this T actor, [NotNull] Equipment equipment)
            where T : IActor, IEquipmentHolder
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));
            Ensure.Any.IsNotNull(equipment, nameof(equipment));

            var action = actor.Actions.Values.FirstOrDefault(a => a is DropAction);

            if (action == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support drop action.");
            }

            var context = new InteractionContext(actor, equipment);

            if (action.AllowedFor(context))
            {
                action.Execute(context);
            }
        }
    }
}
