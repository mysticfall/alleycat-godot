using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using LanguageExt;

namespace AlleyCat.Character
{
    [AutowireContext]
    public class RaceRegistryFactory : GameObjectFactory<RaceRegistry>
    {
        [Service]
        public IEnumerable<Race> Races { get; set; } = Prelude.Seq<Race>();

        protected override Validation<string, RaceRegistry> CreateService() => new RaceRegistry(Races);
    }
}
