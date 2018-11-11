using LanguageExt;

namespace AlleyCat.Character
{
    public interface IRaceRegistry
    {
        Map<string, Race> Races { get; }
    }
}
