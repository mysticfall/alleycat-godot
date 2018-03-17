using System;
using System.Collections.Generic;
using AlleyCat.Common;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public interface IMorphSet : IDirectory<IMorph>, IDisposable
    {
        [NotNull]
        IEnumerable<IMorphGroup> Groups { get; }

        [NotNull]
        IObservable<IMorph> OnMorph { get; }
    }
}
