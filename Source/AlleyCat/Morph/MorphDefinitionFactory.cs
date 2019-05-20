using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public abstract class MorphDefinitionFactory<TDef, TVal> : GameObjectFactory<TDef>
        where TDef : MorphDefinition<TVal>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Export]
        public TVal Default { get; set; }

        [Export]
        public bool Hidden { get; set; }

        protected override Validation<string, TDef> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateService(key, displayName, Hidden, loggerFactory);
        }

        protected abstract Validation<string, TDef> CreateService(
            string key, string displayName, bool hidden, ILoggerFactory loggerFactory);
    }
}
