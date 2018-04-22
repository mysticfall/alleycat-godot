using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IIdentifiable
    {
        [NotNull]
        string Key { get; }
    }

    public static class IdentifiableExtensions
    {
        [NotNull]
        public static IDictionary<string, T> ToDictionary<T>([NotNull] this IEnumerable<T> items)
            where T : IIdentifiable
        {
            Ensure.Any.IsNotNull(items, nameof(items));

            return items.ToDictionary(i => i.Key);
        }
    }
}
