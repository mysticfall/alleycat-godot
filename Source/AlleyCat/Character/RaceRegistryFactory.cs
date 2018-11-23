using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    [AutowireContext]
    public class RaceRegistryFactory : GameObjectFactory<RaceRegistry>
    {
        [Service]
        public IEnumerable<Race> Races { get; set; } = Prelude.Seq<Race>();

        protected override Validation<string, RaceRegistry> CreateService(ILogger logger) =>
            new RaceRegistry(Races, logger);
    }
}
