using System;
using System.Collections.Generic;
using LanguageExt;

namespace AlleyCat.Character.Morph
{
    public interface IMorphSet : IDisposable
    {
        Map<string, IMorph> Morphs { get; }

        IEnumerable<IMorphGroup> Groups { get; }

        IObservable<IMorph> OnMorph { get; }

        IEnumerable<IMorph> GetMorphs(IMorphGroup group);
    }
}
