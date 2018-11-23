using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class RaceRegistry : GameObject, IRaceRegistry
    {
        public Map<string, Race> Races { get; }

        public RaceRegistry(IEnumerable<Race> races, ILogger logger) : base(logger)
        {
            Ensure.That(races, nameof(races)).IsNotNull();

            Races = races.ToMap();
        }
    }
}
