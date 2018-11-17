using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public static class OptionalExtensions
    {
        public static Option<T> Flatten<T>(this Option<Option<T>> option) => option.Bind(identity);
    }
}
