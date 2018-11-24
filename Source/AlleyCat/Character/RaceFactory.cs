using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class RaceFactory : BaseRaceFactory<Race>
    {
        protected override Validation<string, Race> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return new Race(key, displayName, EquipmentSlots, loggerFactory);
        }
    }
}
