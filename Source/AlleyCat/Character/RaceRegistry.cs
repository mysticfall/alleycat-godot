using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class RaceRegistry : GameObject, IRaceRegistry
    {
        public Map<string, Race> Races { get; }

        public RaceRegistry(IEnumerable<Race> races, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(races, nameof(races)).IsNotNull();

            Races = races.ToMap();

            Races.Values.Iter(race => this.LogInfo("Found race: '{}'.", race));
        }
    }
}
