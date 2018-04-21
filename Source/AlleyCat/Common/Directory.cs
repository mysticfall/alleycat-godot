using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public abstract class Directory<T> : AutowiredNode, IDirectory<T> where T : IIdentifiable
    {
        protected virtual Node ItemsParent => this;

        protected IDictionary<string, T> Cache => _cache ?? (_cache = CreateCache());

        private IDictionary<string, T> _cache;

        private IDictionary<string, T> CreateCache() => ItemsParent.GetChildren<T>().ToDictionary(GetKey);

        public IEnumerator<T> GetEnumerator() => Cache.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => Cache.Count;

        public bool ContainsKey(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            return Cache.ContainsKey(key);
        }

        public T this[string key]
        {
            get
            {
                Ensure.Any.IsNotNull(key, nameof(key));

                Cache.TryGetValue(key, out var item);

                return item;
            }
        }

        protected virtual string GetKey(T item) => item.Key;

        protected void ClearCache() => _cache = null;
    }
}
