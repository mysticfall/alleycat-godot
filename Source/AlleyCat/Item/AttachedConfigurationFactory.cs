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
            ILoggerFactory loggerFactory)
        {
            return new AttachedConfiguration(
                key,
                slot,
                additionalSlots,
                Tags,
                Active, 
                loggerFactory)
            {
                Mesh = Mesh,
                Animation = Animation,
                AnimationBlend = AnimationBlend,
                AnimationTransition = AnimationTransition
            };
        }
    }
}
