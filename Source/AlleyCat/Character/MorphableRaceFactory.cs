using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Morph;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class MorphableRaceFactory : BaseRaceFactory<MorphableRace>
    {
        [Node("Morphs")]
        protected virtual IEnumerable<Node> MorphRoots { get; set; }

        protected override Validation<string, MorphableRace> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            Option<Sex> ParseSex(string name) => Enum.TryParse(name, out Sex sex) ? Prelude.Some(sex) : Prelude.None;

            var groups = MorphRoots
                .Bind(r => ParseSex(r.Name).Map(sex => (sex, r.GetChildComponents<IMorphGroup>())));

            return new MorphableRace(key, displayName, EquipmentSlots, Prelude.toMap(groups), loggerFactory);
        }
    }
}
