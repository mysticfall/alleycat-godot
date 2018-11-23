using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
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

        [Service]
        public IEnumerable<IMorphDefinition> Definitions { get; set; } = Seq<IMorphDefinition>();

        protected override Validation<string, MorphGroup> CreateService(ILogger logger)
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return new MorphGroup(key, displayName, Definitions, logger);
        }
    }
}
