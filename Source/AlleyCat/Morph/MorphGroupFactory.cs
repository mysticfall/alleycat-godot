using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    [AutowireContext]
    public class MorphGroupFactory : GameObjectFactory<MorphGroup>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Service(local: true)]
        public IEnumerable<IMorphDefinition> Definitions { get; set; } = Seq<IMorphDefinition>();

        protected override Validation<string, MorphGroup> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return new MorphGroup(key, displayName, Definitions, loggerFactory);
        }
    }
}
