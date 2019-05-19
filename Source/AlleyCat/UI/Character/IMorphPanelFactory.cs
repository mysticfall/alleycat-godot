using AlleyCat.Game;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using LanguageExt;

namespace AlleyCat.UI.Character
{
    public interface IMorphPanelFactory : IGameObjectFactory
    {
        Option<IMorph> Morph { get; set; }
    }

    namespace Generic
    {
        public interface IMorphPanelFactory<TVal, TDef> : IMorphPanelFactory where TDef : IMorphDefinition
        {
            new Option<IMorph<TVal, TDef>> Morph { get; set; }
        }
    }
}
