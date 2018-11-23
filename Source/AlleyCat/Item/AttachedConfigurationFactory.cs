using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class AttachedConfigurationFactory : EquipmentConfigurationFactory<AttachedConfiguration>
    {
        protected override Validation<string, AttachedConfiguration> CreateService(
            string key, string slot, Set<string> additionalSlots, ILogger logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(slot, nameof(slot)).IsNotNullOrEmpty();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return new AttachedConfiguration(
                key,
                slot,
                additionalSlots,
                Tags,
                Active, 
                logger)
            {
                Mesh = Mesh,
                Animation = Animation,
                AnimationBlend = AnimationBlend,
                AnimationTransition = AnimationTransition
            };
        }
    }
}
