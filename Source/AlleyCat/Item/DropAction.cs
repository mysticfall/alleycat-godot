using System;
using AlleyCat.Action;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class DropAction : EquipmentAction
    {
        protected override void DoExecute(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            var scene = holder.Spatial.GetCurrentScene();

            holder.Unequip(equipment, scene.ItemsRoot);
        }

        protected override bool AllowedFor(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            return holder.HasEquipment(equipment.Slot) && equipment.Configuration.HasTag(Carry);
        }
    }

    public static class DropActionExtensions
    {
        public static void Drop<T>(this T actor, Equipment equipment)
            where T : class, IActor, IEquipmentHolder
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            var action = actor.Actions.Values.Find(a => a is DropAction);

            action.Match(
                a => a.Execute(new InteractionContext(actor, equipment)),
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor), "The specified actor does not support drop action.")
            );
        }
    }
}
