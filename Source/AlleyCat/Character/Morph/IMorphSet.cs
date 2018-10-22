using System;
using System.Collections.Generic;

namespace AlleyCat.Character.Morph
{
    public interface IMorphSet : IReadOnlyDictionary<string, IMorph>, IDisposable
    {
        IEnumerable<IMorphGroup> Groups { get; }

        IObservable<IMorph> OnMorph { get; }
    }
}
