using System.Diagnostics;
using System.Linq;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public interface IEquipmentContainer : ISlotContainer<EquipmentSlot, Equipment>
    {
    }

    public static class EquipmentContainerExtensions
    {
        public static Option<EquipmentConfiguration> FindConfiguration(
            this IEquipmentContainer container, Equipment item, Set<string> tags)
        {
            Ensure.That(container, nameof(container)).IsNotNull();
            Ensure.That(item, nameof(item)).IsNotNull();

            var allConfigs = item.Configurations.Values;

            return (tags.Any() ? allConfigs.TaggedAny(tags) : allConfigs).Find(container.AllowedFor);
        }

        public static Option<Equipment> Equip(
            this IEquipmentContainer container, Equipment item, Set<string> tags)
        {
            return FindConfiguration(container, item, tags).Map(c => Equip(container, item, c));
        }

        public static Equipment Equip(
            this IEquipmentContainer container, Equipment item, EquipmentConfiguration configuration)
        {
            Ensure.That(container, nameof(container)).IsNotNull();
            Ensure.That(configuration, nameof(configuration)).IsNotNull();

            configuration.Activate();
            container.Add(item);

            return item;
        }

        public static Equipment Unequip(this IEquipmentContainer container, Equipment item) =>
            Unequip(container, item, None);

        public static Equipment Unequip(
            this IEquipmentContainer container, Equipment item, Option<Node> dropTo)
        {
            Ensure.That(container, nameof(container)).IsNotNull();
            Ensure.That(item, nameof(item)).IsNotNull();

            var parent = dropTo.IfNone(() => item.Node.GetTree().CurrentScene);

            Debug.Assert(parent != null, "parent != null");

            var transform = item.GetGlobalTransform();

            container.Remove(item);

            item.Configuration?.Deactivate();

            parent.AddChild(item.Node);

            item.SetGlobalTransform(transform);

            return item;
        }

        public static Option<Equipment> Unequip(this IEquipmentContainer container, string slot) =>
            Unequip(container, slot, None);

        public static Option<Equipment> Unequip(
            this IEquipmentContainer container, string slot, Option<Node> dropTo)
        {
            Ensure.Any.IsNotNull(container, nameof(container));

            return container.FindItem(slot).Map(i => Unequip(container, i, dropTo));
        }
    }
}
