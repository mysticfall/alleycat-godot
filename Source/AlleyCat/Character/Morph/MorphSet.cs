using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
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

        public MorphSet([NotNull] IEnumerable<IMorph> morphs)
        {
            Ensure.Any.IsNotNull(morphs, nameof(morphs));

            var list = morphs.ToList();

            _morphs = list.ToDictionary(m => m.Key);

            Groups = list.Select(m => m.Definition.Group).Distinct().ToList();
            OnMorph = list.Select(m => m.OnChange.Select(_ => m)).Merge();
        }

        public IEnumerator<IMorph> GetEnumerator() => _morphs.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool ContainsKey(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            return _morphs.ContainsKey(key);
        }

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

            foreach (var morph in this)
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
            foreach (var morph in this)
            {
                morph.Dispose();
            }
        }
    }
}
