using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public interface IEquipmentHolder : IRigged, IMarkable
    {
        IEquipmentContainer Equipments { get; }
    }

    public static class EquipmentHolderExtensions
    {
        public static bool HasEquipment(this IEquipmentHolder holder, string slot) =>
            FindEquipment(holder, slot).IsSome;

        public static Option<Equipment> FindEquipment(this IEquipmentHolder holder, string slot)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Items.Find(slot);
        }

        public static Option<EquipmentConfiguration> FindEquipConfiguration(
            this IEquipmentHolder holder, Equipment item, params string[] tags)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.FindConfiguration(item, tags);
        }

        public static Option<Equipment> Equip(
            this IEquipmentHolder holder, Equipment item, params string[] tags)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Equip(item, tags);
        }

        public static Equipment Equip(
            this IEquipmentHolder holder, Equipment item, EquipmentConfiguration configuration)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Equip(item, configuration);
        }

        public static Option<Equipment> Unequip(
            this IEquipmentHolder holder, Equipment item) => Unequip(holder, item, None);

        public static Option<Equipment> Unequip(
            this IEquipmentHolder holder, Equipment item, Option<Node> dropTo)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Unequip(item, dropTo);
        }

        public static Option<Equipment> Unequip(this IEquipmentHolder holder, string slot) =>
            Unequip(holder, slot, None);

        public static Option<Equipment> Unequip(
            this IEquipmentHolder holder, string slot, Option<Node> dropTo)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Unequip(slot, dropTo);
        }
    }
}
