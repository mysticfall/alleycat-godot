using System;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    public interface ICharacterAware<T> where T : ICharacter
    {
        [CanBeNull]
        T Character { get; set; }

        IObservable<T> OnCharacterChange { get; }
    }
}
