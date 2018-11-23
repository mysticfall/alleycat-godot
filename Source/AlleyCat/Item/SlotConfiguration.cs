using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public abstract class SlotConfiguration : GameObject, ISlotConfiguration
    {
        public string Key { get; }

        public string Slot { get; }

        public Set<string> AdditionalSlots { get; }

        protected SlotConfiguration(string key, string slot, Set<string> additionalSlots, ILogger logger) : base(logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(slot, nameof(slot)).IsNotNullOrEmpty();

            Key = key;
            Slot = slot;
            AdditionalSlots = additionalSlots;
        }
    }
}
