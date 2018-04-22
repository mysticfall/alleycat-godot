using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.Common
{
    public abstract class Directory<T> : AutowiredNode, IReadOnlyDictionary<string, T> where T : IIdentifiable
    {
        public int Count => Cache.Count;

        public IEnumerable<string> Keys => Cache.Keys;

        public IEnumerable<T> Values => Cache.Values;

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => Cache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Cache.GetEnumerator();

        protected virtual Node ItemsParent => this;

        protected IDictionary<string, T> Cache => _cache ?? (_cache = CreateCache());

        private IDictionary<string, T> _cache;

        private IDictionary<string, T> CreateCache() => ItemsParent.GetChildren<T>().ToDictionary(GetKey);

        public bool ContainsKey(string key) => Cache.ContainsKey(key);

        public bool TryGetValue(string key, out T value) => Cache.TryGetValue(key, out value);

        public T this[string key] => Cache[key];

        protected virtual string GetKey(T item) => item.Key;

        protected void ClearCache() => _cache = null;
    }
}
