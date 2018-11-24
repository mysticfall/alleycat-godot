using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class EquipmentSlotFactory : SlotFactory<EquipmentSlot>
    {
        [Export]
        public EquipType EquipType { get; set; }

        protected override Validation<string, EquipmentSlot> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return new EquipmentSlot(key, displayName, EquipType, AllowedFor, loggerFactory);
        }
    }
}
