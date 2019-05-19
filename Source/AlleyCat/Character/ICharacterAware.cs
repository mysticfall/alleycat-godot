using System;
using AlleyCat.Autowire;
using LanguageExt;

namespace AlleyCat.Character
{
    [NonInjectable]
    public interface ICharacterAware<T> where T : ICharacter
    {
        Option<T> Character { get; set; }

        IObservable<Option<T>> OnCharacterChange { get; }
    }
}
