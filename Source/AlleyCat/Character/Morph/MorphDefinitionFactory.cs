using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character.Morph
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

        protected override Validation<string, TDef> CreateService(ILogger logger)
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateService(key, displayName, logger);
        }

        protected abstract Validation<string, TDef> CreateService(string key, string displayName, ILogger logger);
    }
}
