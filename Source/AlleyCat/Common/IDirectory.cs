using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IDirectory<out T> : IReadOnlyCollection<T>
    {
        bool ContainsKey([NotNull] string key);

        [CanBeNull]
        T this[[NotNull] string key] { get; }
    }
}
