using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.IO;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    [Singleton(typeof(IMorphSet))]
    public class MorphSet : IMorphSet
    {
        public string Key => "Morphs";

        public IEnumerable<IMorphGroup> Groups { get; }

        public IObservable<IMorph> OnMorph { get; }

        private readonly IDictionary<string, IMorph> _morphs;

        public int Count => _morphs.Count;

        public IEnumerable<string> Keys => _morphs.Keys;

        public IEnumerable<IMorph> Values => _morphs.Values;

        public IEnumerator<KeyValuePair<string, IMorph>> GetEnumerator() => _morphs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _morphs.GetEnumerator();

        public MorphSet([NotNull] IEnumerable<IMorph> morphs)
        {
            Ensure.Any.IsNotNull(morphs, nameof(morphs));

            var list = morphs.ToList();

            list.ForEach(m => m.Apply());

            _morphs = list.ToDictionary();

            Groups = list.Select(m => m.Definition.Group).Distinct().ToList();
            OnMorph = list.Select(m => m.OnChange.Select(_ => m)).Merge();
        }

        public bool ContainsKey(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            return _morphs.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IMorph value) => _morphs.TryGetValue(key, out value);

        public IMorph this[string key]
        {
            get
            {
                Ensure.Any.IsNotNull(key, nameof(key));

                return _morphs[key];
            }
        }

        public virtual void SaveState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            this.ToList().ForEach(m => state[m.Key] = m.Value);
        }

        public virtual void RestoreState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            foreach (var morph in Values)
            {
                if (state.ContainsKey(morph.Key))
                {
                    morph.Value = state[morph.Key];
                }
                else
                {
                    morph.Reset();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_morphs == null) return;

            foreach (var morph in _morphs.Values)
            {
                morph?.Dispose();
            }
        }
    }
}
