using EnsureThat;
using LanguageExt;

namespace AlleyCat.Character
{
    public class RaceFactory : BaseRaceFactory<Race>
    {
        protected override Validation<string, Race> CreateService(string key, string displayName)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            return new Race(key, displayName, EquipmentSlots);
        }
    }
}
