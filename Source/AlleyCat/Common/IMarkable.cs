using AlleyCat.Autowire;
using LanguageExt;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IMarkable
    {
        Map<string, Marker> Markers { get; }
    }
}
