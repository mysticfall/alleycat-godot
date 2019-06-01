using System.Linq;
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
        public static bool HasEquipment(this IEquipmentHolder holder, IEquipment equipment)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            return holder.Equipments.Items.Values.Contains(equipment);
        }

        public static Option<EquipmentConfiguration> FindEquipConfiguration(
            this IEquipmentHolder holder, IEquipment item, Set<string> tags)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.FindConfiguration(item, tags);
        }

        public static Option<IEquipment> Equip(
            this IEquipmentHolder holder, IEquipment item, Set<string> tags)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Equip(item, tags);
        }

        public static IEquipment Equip(
            this IEquipmentHolder holder, IEquipment item, EquipmentConfiguration configuration)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Equip(item, configuration);
        }

        public static IEquipment Unequip(this IEquipmentHolder holder, IEquipment item) => Unequip(holder, item, None);

        public static IEquipment Unequip(
            this IEquipmentHolder holder, IEquipment item, Option<Node> dropTo)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Unequip(item, dropTo);
        }

        public static Option<IEquipment> Unequip(this IEquipmentHolder holder, string slot) =>
            Unequip(holder, slot, None);

        public static Option<IEquipment> Unequip(
            this IEquipmentHolder holder, string slot, Option<Node> dropTo)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return holder.Equipments.Unequip(slot, dropTo);
        }
    }
}
