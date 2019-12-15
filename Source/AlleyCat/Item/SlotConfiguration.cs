using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public abstract class SlotConfiguration : GameNode, ISlotConfiguration
    {
        public string Key { get; }

        public string Slot { get; }

        public Set<string> AdditionalSlots { get; }

        protected SlotConfiguration(
            string key, 
            string slot, 
            Set<string> additionalSlots, 
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(slot, nameof(slot)).IsNotNullOrEmpty();

            Key = key;
            Slot = slot;
            AdditionalSlots = additionalSlots;
        }
    }
}
