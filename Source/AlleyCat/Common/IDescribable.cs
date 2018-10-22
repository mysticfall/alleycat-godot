using LanguageExt;

namespace AlleyCat.Common
{
    public interface IDescribable
    {
        Option<string> Description { get; }
    }
}
