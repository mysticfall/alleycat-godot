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
    public interface IEquipmentContainer : ISlotContainer<EquipmentSlot, IEquipment>
    {
    }

    public static class EquipmentContainerExtensions
    {
        public static Option<EquipmentConfiguration> FindConfiguration(
            this IEquipmentContainer container, IEquipment item, Set<string> tags)
        {
            Ensure.That(container, nameof(container)).IsNotNull();
            Ensure.That(item, nameof(item)).IsNotNull();

            var allConfigs = item.Configurations.Values;

            return (tags.Any() ? allConfigs.TaggedAny(tags) : allConfigs).Find(container.AllowedFor);
        }

        public static Option<IEquipment> Equip(
            this IEquipmentContainer container, IEquipment item, Set<string> tags)
        {
            return FindConfiguration(container, item, tags).Map(c => Equip(container, item, c));
        }

        public static IEquipment Equip(
            this IEquipmentContainer container, IEquipment item, EquipmentConfiguration configuration)
        {
            Ensure.That(container, nameof(container)).IsNotNull();
            Ensure.That(configuration, nameof(configuration)).IsNotNull();

            configuration.Activate();
            container.Add(item);

            return item;
        }

        public static IEquipment Unequip(this IEquipmentContainer container, IEquipment item) =>
            Unequip(container, item, None);

        public static IEquipment Unequip(
            this IEquipmentContainer container, IEquipment item, Option<Node> dropTo)
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

        public static Option<IEquipment> Unequip(this IEquipmentContainer container, string slot) =>
            Unequip(container, slot, None);

        public static Option<IEquipment> Unequip(
            this IEquipmentContainer container, string slot, Option<Node> dropTo)
        {
            Ensure.Any.IsNotNull(container, nameof(container));

            return container.FindItemInSlot(slot).Map(i => Unequip(container, i, dropTo));
        }
    }
}
