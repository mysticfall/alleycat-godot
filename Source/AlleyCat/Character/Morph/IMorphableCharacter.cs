namespace AlleyCat.Character.Morph
{
    public interface IMorphableCharacter : ICharacter, IMorphable
    {
        new IMorphableRace Race { get; }
    }
}
