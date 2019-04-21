using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class AttachedConfigurationFactory : EquipmentConfigurationFactory<AttachedConfiguration>
    {
        protected override Validation<string, AttachedConfiguration> CreateService(
            string key, 
            string slot, 
            Set<string> additionalSlots, 
            Set<string> tags, 
            ILoggerFactory loggerFactory)
        {
            return new AttachedConfiguration(
                key,
                slot,
                additionalSlots,
                tags,
                Active, 
                loggerFactory)
            {
                Mesh = Mesh,
                EquipAnimation = EquipAnimation,
                UnequipAnimation = UnequipAnimation,
                Animation = Animation,
                AnimationBlend = AnimationBlend,
                AnimationTransition = AnimationTransition
            };
        }
    }
}
