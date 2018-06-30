using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface IEquipmentHolder : IRigged, IMarkable
    {
        IEquipmentContainer Equipments { get; }
    }

    public static class EquipmentHolderExtensions
    {
        public static bool HasEquipment([NotNull] this IEquipmentHolder holder, [NotNull] string slot) =>
            GetEquipment(holder, slot) != null;

        [CanBeNull]
        public static Equipment GetEquipment([NotNull] this IEquipmentHolder holder, [NotNull] string slot)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));
            Ensure.Any.IsNotNull(slot, nameof(slot));

            return holder.Equipments.TryGetValue(slot, out var value) ? value : null;
        }

        [CanBeNull]
        public static EquipmentConfiguration FindEquipConfiguration(
            [NotNull] this IEquipmentHolder holder,
            [NotNull] Equipment item,
            params string[] tags)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            return holder.Equipments.FindConfiguration(item, tags);
        }

        [CanBeNull]
        public static Equipment Equip(
            [NotNull] this IEquipmentHolder holder,
            [NotNull] Equipment item,
            params string[] tags)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            return holder.Equipments.Equip(item, tags);
        }

        [NotNull]
        public static Equipment Equip(
            [NotNull] this IEquipmentHolder holder,
            [NotNull] Equipment item,
            [NotNull] EquipmentConfiguration configuration)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            return holder.Equipments.Equip(item, configuration);
        }

        [NotNull]
        public static Equipment Unequip(
            [NotNull] this IEquipmentHolder holder,
            [NotNull] Equipment item)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            return holder.Equipments.Unequip(item);
        }

        [CanBeNull]
        public static Equipment Unequip(
            [NotNull] this IEquipmentHolder holder,
            [NotNull] string slot)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            return holder.Equipments.Unequip(slot);
        }
    }
}
