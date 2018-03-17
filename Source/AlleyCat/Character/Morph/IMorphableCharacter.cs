using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public interface IMorphableCharacter : ICharacter, IMorphable
    {
        new IMorphableRace Race { get; }

        void SwitchSex([NotNull] string race);

        void SwitchRace(Sex sex);
    }
}
