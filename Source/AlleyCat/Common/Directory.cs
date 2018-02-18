using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public abstract class Directory<T> : Node, IDirectory<T> where T : IIdentifiable
    {
        private IDictionary<string, T> Cache => _cache ?? (_cache = CreateCache());

        private IDictionary<string, T> _cache;

        private IDictionary<string, T> CreateCache() => 
            this.GetChildren<T>().ToDictionary(c => c.Key);

        public override void _Ready()
        {
            base._Ready();

            _cache = CreateCache();
        }

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

                return Cache[key];
            }
        }
    }
}
