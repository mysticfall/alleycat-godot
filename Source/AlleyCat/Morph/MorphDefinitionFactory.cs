using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;

namespace AlleyCat.Morph
{
    public abstract class MorphDefinitionFactory<TDef, TVal> : GameResourceFactory<TDef>
        where TDef : MorphDefinition<TVal>
    {
        [Export]
        public string DisplayName { get; set; }

        [Export]
        public TVal Default { get; set; }

        [Export]
        public bool Hidden { get; set; }

        protected override Validation<string, TDef> CreateResource()
        {
            return ValidateName.Bind(key =>
                CreateResource(key, DisplayName.TrimToOption().Map(Tr).IfNone(key), Hidden));
        }

        protected abstract Validation<string, TDef> CreateResource(string key, string displayName, bool hidden);
    }
}
