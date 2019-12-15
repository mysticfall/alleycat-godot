using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class MorphGroupFactory : GameResourceFactory<MorphGroup>
    {
        [Export]
        public string DisplayName { get; set; }

        [Export]
        public IEnumerable<Resource> Morphs { get; set; }

        protected override Validation<string, MorphGroup> CreateResource()
        {
            var definitions = Optional(Morphs)
                .Flatten()
                .Map(m => m.Validate<IMorphDefinition>())
                .Sequence();

            return
                from key in ValidateName
                from morphs in definitions
                select new MorphGroup(key, DisplayName.TrimToOption().Map(Tr).IfNone(key), morphs);
        }
    }
}
