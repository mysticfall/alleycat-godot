using System;
using System.Diagnostics;
using AlleyCat.Action;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using static LanguageExt.Prelude;
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

            var scene = GetTree().CurrentScene;
            var path = ((IScene) scene).ItemsPath;

            Debug.Assert(path != null, "path != null");

            var parent = Optional(scene.GetNode(path)).IfNone(scene);

            holder.Unequip(equipment, parent);
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
