using EnsureThat;
using Godot;

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
        public static string DisplayName(this EquipmentType type, Node context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context.Tr("equipment.type." + type);
        }
    }
}
