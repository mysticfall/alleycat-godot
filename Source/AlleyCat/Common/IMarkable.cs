using LanguageExt;

namespace AlleyCat.Common
{
    public interface IMarkable
    {
        Map<string, Marker> Markers { get; }
    }
}
