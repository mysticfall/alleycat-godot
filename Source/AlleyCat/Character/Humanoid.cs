using System;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Event;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Character
{
    public class Humanoid : Character<IPairedEyeSight, ILocomotion>, IHumanoid
    {
        public override IRace Race => RaceRegistry?[_race];

        IMorphableRace IMorphableCharacter.Race => (IMorphableRace) Race;

        public override Sex Sex => _sex;

        public IMorphSet Morphs => _morphSet.Value;

        public IObservable<IMorphSet> OnMorphsChange => _morphSet;

        private readonly IReactiveProperty<IMorphSet> _morphSet = new ReactiveProperty<IMorphSet>();

        [Export, UsedImplicitly] private string _race;

        [Export, UsedImplicitly] private Sex _sex;

        public void SwitchRace(Sex sex) => Switch(_race, sex);

        public void SwitchSex(string race) => Switch(race, _sex);

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

            _sex = sex;
            _race = race.Key;

            var groups = race.GetMorphGroups(sex).ToList();
            var morphs = groups.SelectMany(g => g.ToList()).Select(d => d.CreateMorph(this));

            _morphSet.Value?.Dispose();
            _morphSet.Value = new MorphSet(morphs);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Switch(_race, _sex);
        }
    }
}
