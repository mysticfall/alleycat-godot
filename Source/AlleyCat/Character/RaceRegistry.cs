using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Character
{
    public class RaceRegistry : GameObject, IRaceRegistry
    {
        public Map<string, Race> Races { get; }

        public RaceRegistry(IEnumerable<Race> races)
        {
            Ensure.That(races, nameof(races)).IsNotNull();

            Races = races.ToMap();
        }
    }
}
