using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

namespace AlleyCat.Character.Morph
{
    [Singleton(typeof(IMorphSet))]
    public class MorphSet : BaseNode, IMorphSet
    {
        public IEnumerable<IMorphGroup> Groups { get; }

        public IObservable<IMorph> OnMorph { get; }

        public int Count => _morphs.Count;

        public IEnumerable<string> Keys => _morphs.Keys;

        public IEnumerable<IMorph> Values => _morphs.Values;

        public IEnumerator<KeyValuePair<string, IMorph>> GetEnumerator() =>
            _morphs.Values.Map(m => new KeyValuePair<string, IMorph>(m.Key, m)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _morphs.GetEnumerator();

        private Map<string, IMorph> _morphs;

        public MorphSet(IEnumerable<IMorph> morphs)
        {
            Ensure.That(morphs, nameof(morphs)).IsNotNull();

            var items = morphs.Freeze();

            items.Iter(m => m.AddTo(this).Apply());

            _morphs = items.ToMap();

            Groups = items.Map(m => m.Definition.Group).Distinct().ToList();
            OnMorph = items.Map(m => m.OnChange.Select(_ => m)).Merge();
        }

        public bool ContainsKey(string key)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            return _morphs.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IMorph value)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            var result = _morphs.Find(key);

            value = result.ValueUnsafe();

            return result.IsSome;
        }

        public IMorph this[string key]
        {
            get
            {
                Ensure.That(key, nameof(key)).IsNotNull();

                return _morphs[key];
            }
        }
    }
}
