using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Common
{
    public interface ITaggable
    {
        Set<string> Tags { get; }

        bool HasTag(string tag);

        void AddTag(string tag);

        void RemoveTag(string tag);
    }

    public static class TaggableExtensions
    {
        public static IEnumerable<T> TaggedAll<T>(
            this IEnumerable<T> items, params string[] tags) where T : ITaggable
        {
            Ensure.That(items, nameof(items)).IsNotNull();

            return items.Filter(i => tags.All(i.HasTag));
        }

        public static IEnumerable<T> TaggedAny<T>(
            this IEnumerable<T> items, params string[] tags) where T : ITaggable
        {
            Ensure.That(items, nameof(items)).IsNotNull();

            return items.Filter(i => tags.Any(i.HasTag));
        }

        public static IEnumerable<T> TaggedMost<T>(
            this IEnumerable<T> items, params string[] tags) where T : ITaggable
        {
            Ensure.That(items, nameof(items)).IsNotNull();

            return items
                .Map(item => (item, count: tags.Count(item.HasTag)))
                .Filter(t => t.count > 0)
                .OrderByDescending(t => t.count)
                .Map(t => t.item);
        }
    }
}
