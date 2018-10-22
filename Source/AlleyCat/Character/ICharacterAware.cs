using System;
using LanguageExt;

namespace AlleyCat.Character
{
    public interface ICharacterAware<T> where T : ICharacter
    {
        Option<T> Character { get; set; }

        IObservable<Option<T>> OnCharacterChange { get; }
    }
}
