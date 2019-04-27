using System;
using System.Collections.Generic;

namespace AlleyCat.Morph
{
    public interface IMorphSet : IReadOnlyDictionary<string, IMorph>, IDisposable
    {
        IEnumerable<IMorphGroup> Groups { get; }

        IObservable<IMorph> OnMorph { get; }

        IEnumerable<IMorph> GetMorphs(IMorphGroup group);
    }
}
