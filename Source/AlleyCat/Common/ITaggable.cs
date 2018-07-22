using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface ITaggable
    {
        [NotNull]
        IEnumerable<string> Tags { get; }

        bool HasTag([NotNull] string tag);

        void AddTag([NotNull] string tag);

        void RemoveTag([NotNull] string tag);
    }

    public static class TaggableExtensions
    {
        [NotNull]
        public static IEnumerable<T> TaggedAll<T>(
            [NotNull] this IEnumerable<T> items, params string[] tags) where T : ITaggable
        {
            Ensure.Any.IsNotNull(items, nameof(items));

            return items.Where(i => tags.All(i.HasTag));
        }

        [NotNull]
        public static IEnumerable<T> TaggedAny<T>(
            [NotNull] this IEnumerable<T> items, params string[] tags) where T : ITaggable
        {
            Ensure.Any.IsNotNull(items, nameof(items));

            return items.Where(i => tags.Any(i.HasTag));
        }

        [NotNull]
        public static IEnumerable<T> TaggedMost<T>(
            [NotNull] this IEnumerable<T> items, params string[] tags) where T : ITaggable
        {
            Ensure.Any.IsNotNull(items, nameof(items));

            return items
                .Select(item => (item, count: tags.Count(item.HasTag)))
                .Where(t => t.count > 0)
                .OrderByDescending(t => t.count)
                .Select(t => t.item);
        }
    }
}
