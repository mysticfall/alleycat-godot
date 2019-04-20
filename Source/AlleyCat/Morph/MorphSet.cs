using System;
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
        public Map<string, IMorph> Morphs { get; }

        public IEnumerable<IMorphGroup> Groups { get; }

        public IObservable<IMorph> OnMorph { get; }

        private readonly Map<string, IEnumerable<IMorph>> _morphsByGroup;

        public MorphSet(IEnumerable<IMorphGroup> groups, IEnumerable<IMorph> morphs)
        {
            Ensure.That(groups, nameof(groups)).IsNotNull();
            Ensure.That(morphs, nameof(morphs)).IsNotNull();

            Groups = groups.Freeze();
            Morphs = morphs.ToMap();

            OnMorph = Morphs.Values.Map(m => m.OnChange.Select(_ => m)).Merge();

            var groupsByMorph = toMap(Groups.Bind(g => g.Definitions.Map(d => (d.Key, g))));

            _morphsByGroup = toMap(
                Morphs.Values
                    .Bind(m => groupsByMorph.Find(m.Key).Map(g => (group: g.Key, morphs: m)))
                    .GroupBy(v => v.group, v => v.morphs)
                    .Map(v => (v.Key, v.AsEnumerable())));

            Morphs.Iter(m => m.Initialize());
        }

        public IEnumerable<IMorph> GetMorphs(IMorphGroup group)
        {
            Ensure.That(group, nameof(group)).IsNotNull();

            return _morphsByGroup.Find(group.Key).Flatten();
        }

        public virtual void Dispose()
        {
            Morphs.Values.Iter(m => m.DisposeQuietly());
        }
    }
}
