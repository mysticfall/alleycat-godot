using AlleyCat.Character;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface ICharacterControl<T> : IControl where T : ICharacter
    {
        [CanBeNull]
        T Character { get; set; }
    }
}
