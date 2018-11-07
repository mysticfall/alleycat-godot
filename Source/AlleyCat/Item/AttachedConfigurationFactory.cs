using EnsureThat;
using LanguageExt;

namespace AlleyCat.Item
{
    public class AttachedConfigurationFactory : EquipmentConfigurationFactory<AttachedConfiguration>
    {
        protected override Validation<string, AttachedConfiguration> CreateService(
            string key, string slot, Set<string> additionalSlots)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(slot, nameof(slot)).IsNotNullOrEmpty();

            return new AttachedConfiguration(
                key,
                slot,
                additionalSlots,
                Tags,
                Active)
            {
                Mesh = Mesh,
                Animation = Animation,
                AnimationBlend = AnimationBlend,
                AnimationTransition = AnimationTransition
            };
        }
    }
}
