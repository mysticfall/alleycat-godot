using System.Collections;
using System.Collections.Generic;
using AlleyCat.Autowire;
using EnsureThat;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public abstract class Directory<T> : AutowiredNode, IReadOnlyDictionary<string, T>
    {
        public int Count => Cache.Count;

        public IEnumerable<string> Keys => Cache.Keys;

        public IEnumerable<T> Values => Cache.Values;

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator() =>
            Cache.ValueTuples.Map(t => new KeyValuePair<string, T>(t.Key, t.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Cache.GetEnumerator();

        protected Map<string, T> Cache => _cache.IfNone(() =>
        {
            _cache = CreateCache();

            return _cache.Head();
        });

        private Option<Map<string, T>> _cache;

        protected virtual Map<string, T> CreateCache() =>
            toMap(this.GetChildComponents<T>().Map(c => (GetKey(c), c)));

        public bool ContainsKey(string key)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            return Cache.ContainsKey(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            value = Cache.Find(key).ValueUnsafe();

            return value != null;
        }

        public T this[string key]
        {
            get
            {
                Ensure.That(key, nameof(key)).IsNotNull();

                return Cache[key];
            }
        }

        protected abstract string GetKey(T item);

        protected void ClearCache() => _cache = None;
    }
}
