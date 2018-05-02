using System;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Event;
using AlleyCat.IO;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

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

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            var groups = ((IMorphableRace) Race)?.GetMorphGroups(Sex).ToList();
            var morphs = groups?.SelectMany(g => g.Values.ToList()).Select(d => d.CreateMorph(this)).ToList();

            if (morphs != null)
            {
                _morphSet.Value = new MorphSet(morphs);
            }
        }

        public override void SaveState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            var transform = state.GetSection("Transform");

            transform["Translation"] = Translation;
            transform["Rotation"] = Rotation;

            var morphs = state.GetSection(Morphs.Key);

            Morphs.SaveState(morphs);
        }

        public override void RestoreState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            var transform = state.GetSection("Transform");

            if (transform.ContainsKey("Translation"))
            {
                Translation = (Vector3) transform["Translation"];
            }

            if (transform.ContainsKey("Rotation"))
            {
                Rotation = (Vector3) transform["Rotation"];
            }

            var morphs = state.GetSection(Morphs.Key);

            Morphs.RestoreState(morphs);
        }
    }
}
