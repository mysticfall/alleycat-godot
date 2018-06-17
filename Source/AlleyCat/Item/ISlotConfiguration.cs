using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface ISlotConfiguration : IIdentifiable
    {
        string Slot { get; }

        IEnumerable<string> AdditionalSlots { get; }
    }

    public static class SlotConfigurationExtensions
    {
        [NotNull]
        public static IEnumerable<string> GetAllSlots([NotNull] this ISlotConfiguration configuration)
        {
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            return new[] {configuration.Slot}.Concat(configuration.AdditionalSlots);
        }
    }
}
