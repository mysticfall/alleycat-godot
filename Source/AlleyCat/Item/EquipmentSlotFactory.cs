using Godot;
using LanguageExt;

namespace AlleyCat.Item
{
    public class EquipmentSlotFactory : SlotFactory<EquipmentSlot>
    {
        [Export]
        public EquipType EquipType { get; set; }

        protected override Validation<string, EquipmentSlot> CreateResource(string key, string displayName)
        {
            return new EquipmentSlot(key, displayName, EquipType, AllowedFor);
        }
    }
}
