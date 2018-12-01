using AlleyCat.Autowire;
using LanguageExt;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IDescribable
    {
        Option<string> Description { get; }
    }
}
