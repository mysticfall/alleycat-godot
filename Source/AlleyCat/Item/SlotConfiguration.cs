using AlleyCat.Common;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Item
{
    public abstract class SlotConfiguration : GameObject, ISlotConfiguration
    {
        public string Key { get; }

        public string Slot { get; }

        public Set<string> AdditionalSlots { get; }

        protected SlotConfiguration(string key, string slot, Set<string> additionalSlots)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(slot, nameof(slot)).IsNotNullOrEmpty();

            Key = key;
            Slot = slot;
            AdditionalSlots = additionalSlots;
        }
    }
}
