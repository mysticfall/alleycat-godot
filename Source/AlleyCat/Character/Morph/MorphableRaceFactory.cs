using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public class MorphableRaceFactory : BaseRaceFactory<MorphableRace>
    {
        [Node("Morphs")]
        protected virtual IEnumerable<Node> MorphRoots { get; set; }

        protected override Validation<string, MorphableRace> CreateService(
            string key, string displayName, ILogger logger)
        {
            Option<Sex> ParseSex(string name) => Enum.TryParse(name, out Sex sex) ? Some(sex) : None;

            var groups = MorphRoots
                .Bind(r => ParseSex(r.Name).Map(sex => (sex, r.GetChildComponents<IMorphGroup>())));

            return new MorphableRace(key, displayName, EquipmentSlots, toMap(groups), logger);
        }
    }
}
