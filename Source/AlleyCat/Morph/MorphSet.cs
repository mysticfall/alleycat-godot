using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class MorphSet : IMorphSet
    {
        public IEnumerable<IMorphGroup> Groups { get; }

        public IObservable<IMorph> OnMorph { get; }

        public IEnumerator<KeyValuePair<string, IMorph>> GetEnumerator() =>
            _morphs.Map(p => new KeyValuePair<string, IMorph>(p.Item1, p.Item2)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _morphs.Count;
        public bool ContainsKey(string key) => _morphs.ContainsKey(key);

        public bool TryGetValue(string key, out IMorph value)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            if (!_morphs.ContainsKey(key))
            {
                value = null;
                return false;
            }

            value = _morphs[key];
            return true;
        }

        public IMorph this[string key] => _morphs[key];

        public IEnumerable<string> Keys => _morphs.Keys;

        public IEnumerable<IMorph> Values => _morphs.Values;

        private readonly Map<string, IMorph> _morphs;

        private readonly Map<string, IEnumerable<IMorph>> _morphsByGroup;

        public MorphSet(IEnumerable<IMorphGroup> groups, IEnumerable<IMorph> morphs)
        {
            Ensure.That(groups, nameof(groups)).IsNotNull();
            Ensure.That(morphs, nameof(morphs)).IsNotNull();

            Groups = groups.Freeze();
            _morphs = morphs.ToMap();

            OnMorph = _morphs.Values.Map(m => m.OnChange.Select(_ => m)).Merge();

            var groupsByMorph = toMap(Groups.Bind(g => g.Map(d => (d.Key, g))));

            _morphsByGroup = toMap(
                _morphs.Values
                    .Bind(m => groupsByMorph.Find(m.Key).Map(g => (group: g.Key, morphs: m)))
                    .GroupBy(v => v.group, v => v.morphs)
                    .Map(v => (v.Key, v.AsEnumerable())));

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            _morphs.Iter(m => m.Initialize());
        }

        public IEnumerable<IMorph> GetMorphs(IMorphGroup group)
        {
            Ensure.That(group, nameof(group)).IsNotNull();

            return _morphsByGroup.Find(group.Key).Flatten();
        }

        public virtual void Dispose() => _morphs.Values.Iter(m => m.DisposeQuietly());
    }
}
