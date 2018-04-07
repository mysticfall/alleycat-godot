using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace AlleyCat.IO
{
    public class DictionaryState : IState
    {
        private readonly IDictionary<string, object> _dictionary;

        public DictionaryState()
        {
            _dictionary = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get
            {
                Ensure.Any.IsNotNull(key, nameof(key));

                return _dictionary.TryGetValue(key, out var value) ? value : null;
            }
            set
            {
                Ensure.Any.IsNotNull(key, nameof(key));

                if (value == null)
                {
                    _dictionary.Remove(key);
                }
                else
                {
                    _dictionary[key] = value;
                }
            }
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<object> Values => _dictionary.Values;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        public bool Contains(KeyValuePair<string, object> item) =>
            ContainsKey(item.Key) && this[item.Key] == item.Value;

        public bool ContainsKey(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            return _dictionary.TryGetValue(key, out value);
        }

        public IState GetSection(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            if (this[key] is IState state) return state;

            var newState = new DictionaryState();

            this[key] = newState;

            return newState;
        }

        public IEnumerable<IState> GetChildren() => _dictionary.Values.OfType<IState>();

        public void Add(string key, object value) => this[key] = value;

        public void Add(KeyValuePair<string, object> item) => this[item.Key] = item.Value;

        public bool Remove(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item) => Remove(item.Key);

        public void Clear() => _dictionary.Clear();

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Ensure.Any.IsNotNull(array, nameof(array));

            _dictionary.CopyTo(array, arrayIndex);
        }
    }
}
