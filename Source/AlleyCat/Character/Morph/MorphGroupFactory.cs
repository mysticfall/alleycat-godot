using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    [AutowireContext]
    public class MorphGroupFactory : GameObjectFactory<MorphGroup>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Service(false)]
        public IEnumerable<IMorphDefinition> Definitions { get; set; } = Seq<IMorphDefinition>();

        protected override Validation<string, MorphGroup> CreateService()
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return new MorphGroup(key, displayName, Definitions);
        }
    }
}
