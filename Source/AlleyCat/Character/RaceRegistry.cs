using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Character
{
    [Singleton(typeof(IRaceRegistry))]
    public class RaceRegistry : IdentifiableDirectory<IRace>, IRaceRegistry
    {
    }
}
