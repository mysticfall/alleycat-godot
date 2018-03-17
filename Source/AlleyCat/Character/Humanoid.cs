using System;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Character
{
    public class Humanoid : Character<IPairedEyeSight, ILocomotion>, IHumanoid
    {
        public new IMorphableRace Race => (IMorphableRace) base.Race;

        public IMorphSet Morphs => _morphSet.Value;

        public IObservable<IMorphSet> OnMorphsChange => _morphSet;

        private readonly IReactiveProperty<IMorphSet> _morphSet = new ReactiveProperty<IMorphSet>();

        public void SwitchRace(Sex sex) => Switch(Race, sex);

        public void SwitchSex(string race) => Switch(race, Sex);

        protected void Switch(string race, Sex sex)
        {
            var value = (IMorphableRace) RaceRegistry[race];

            Ensure.Any.IsNotNull(value, nameof(value));

            Debug.Assert(value != null, $"Unknown race name '{value}'.");

            Switch(value, sex);
        }

        protected virtual void Switch([NotNull] IMorphableRace race, Sex sex)
        {
            Ensure.Any.IsNotNull(race, nameof(race));

            var groups = race.GetMorphGroups(sex).ToList();
            var morphs = groups.SelectMany(g => g.ToList()).Select(d => d.CreateMorph(this));

            _morphSet.Value?.Dispose();
            _morphSet.Value = new MorphSet(morphs);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Switch(Race, Sex);
        }
    }
}
