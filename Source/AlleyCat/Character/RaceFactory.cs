using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class RaceFactory : BaseRaceFactory<Race>
    {
        protected override Validation<string, Race> CreateService(string key, string displayName, ILogger logger) =>
            new Race(key, displayName, EquipmentSlots, logger);
    }
}
