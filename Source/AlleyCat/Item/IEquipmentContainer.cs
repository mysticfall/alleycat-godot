using System.Linq;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface IEquipmentContainer : ISlotContainer<EquipmentSlot, Equipment>
    {
    }

    public static class EquipmentContainerExtensions
    {
        [CanBeNull]
        public static Equipment Equip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] EquippableItem item,
            params string[] tags)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(item, nameof(item));

            var allConfigs = item.Configurations.Values;
            var configuration =
                (tags.Any() ? allConfigs.TaggedAny(tags) : allConfigs).FirstOrDefault(container.AllowedFor);

            return configuration == null ? null : Equip(container, item, configuration);
        }

        [NotNull]
        public static Equipment Equip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] EquippableItem item,
            [NotNull] EquipConfiguration configuration,
            bool dispose = true)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(item, nameof(item));
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            var equipment = configuration.CreateEquipment();

            container.Add(equipment);

            if (dispose)
            {
                item.QueueFree();
            }

            return equipment;
        }

        [NotNull]
        public static EquippableItem Unequip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] Equipment equipment,
            bool dispose = true)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(equipment, nameof(equipment));

            container.Remove(equipment);

            var item = equipment.CreateItem();

            if (dispose)
            {
                equipment.QueueFree();
            }

            return item;
        }
    }
}
