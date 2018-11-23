using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class RaceFactory : BaseRaceFactory<Race>
    {
        protected override Validation<string, Race> CreateService(
            string key, string displayName, ILogger logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return new Race(key, displayName, EquipmentSlots, logger);
        }
    }
}
