using System;

namespace AlleyCat.Character.Morph
{
    public interface IMorphable
    {
        IMorphSet Morphs { get; }

        IObservable<IMorphSet> OnMorphsChange { get; }
    }
}
