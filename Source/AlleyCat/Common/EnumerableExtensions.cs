using System.Collections.Generic;
using EnsureThat;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            Ensure.That(enumerable, nameof(enumerable)).IsNotNull();

            return enumerable.Bind(identity);
        }
    }
}
