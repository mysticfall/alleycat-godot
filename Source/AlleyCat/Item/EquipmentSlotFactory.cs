using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Item
{
    public class EquipmentSlotFactory : SlotFactory<EquipmentSlot>
    {
        [Export]
        public EquipType EquipType { get; set; }

        protected override Validation<string, EquipmentSlot> CreateService(string key, string displayName)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            return new EquipmentSlot(key, displayName, EquipType, AllowedFor);
        }
    }
}
