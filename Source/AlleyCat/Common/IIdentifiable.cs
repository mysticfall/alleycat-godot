using System.Collections.Generic;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public interface IIdentifiable
    {
        string Key { get; }
    }

    public static class IdentifiableExtensions
    {
        public static Map<string, T> ToMap<T>(this IEnumerable<T> items) where T : IIdentifiable
        {
            Ensure.That(items, nameof(items)).IsNotNull();

            return toMap(items.Map(i => (i.Key, i)));
        }
    }
}
