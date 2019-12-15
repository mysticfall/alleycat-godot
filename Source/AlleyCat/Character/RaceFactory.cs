using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    public class RaceFactory : BaseRaceFactory<Race>
    {
        protected override Validation<string, Race> CreateResource(string key, string displayName)
        {
            var slots = Optional(EquipmentSlots)
                .Flatten()
                .Map(s => s.Service)
                .Sequence();

            return slots.Map(s => new Race(key, displayName, s));
        }
    }
}
