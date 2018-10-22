using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public interface ISlotConfiguration : IIdentifiable
    {
        string Slot { get; }

        Set<string> AdditionalSlots { get; }
    }

    public static class SlotConfigurationExtensions
    {
        public static Set<string> GetAllSlots(this ISlotConfiguration configuration)
        {
            Ensure.That(configuration, nameof(configuration)).IsNotNull();

            return Set(configuration.Slot).Union(configuration.AdditionalSlots);
        }
    }
}
