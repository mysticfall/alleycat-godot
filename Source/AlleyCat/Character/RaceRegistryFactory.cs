using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Game;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    [AutowireContext]
    public class RaceRegistryFactory : GameObjectFactory<RaceRegistry>
    {
        [Service]
        public IEnumerable<Race> Races { get; set; } = Seq<Race>();

        protected override Validation<string, RaceRegistry> CreateService(ILoggerFactory loggerFactory) =>
            new RaceRegistry(Races, loggerFactory);
    }
}
