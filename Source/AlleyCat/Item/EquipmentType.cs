using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public enum EquipmentType
    {
        Clothes,
        Accessory,
        Weapon,
        Tool
    }

    public static class EquipmentTypeExtensions
    {
        public static string DisplayName(this EquipmentType type, [NotNull] Node context)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            return context.Tr("equipment.type." + type);
        }
    }
}
