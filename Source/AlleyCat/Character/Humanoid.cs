using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using LanguageExt;

namespace AlleyCat.Character
{
    public class Humanoid : Character<IPairedEyeSight, ILocomotion>, IHumanoid
    {
        public override IRace Race => RaceRegistry[_race];

        IMorphableRace IMorphableCharacter.Race => (IMorphableRace) Race;

        public override Sex Sex => _sex;

        public IMorphSet Morphs => _morphSet.Head();

        public override bool Valid => base.Valid && _morphSet.IsSome && !string.IsNullOrWhiteSpace(_race);

        private Option<IMorphSet> _morphSet;

        [Export] private string _race;

        [Export] private Sex _sex;

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            var groups = ((IMorphableRace) Race).GetMorphGroups(Sex);
            var morphs = groups.SelectMany(g => g.Values).Select(d => d.CreateMorph(this));

            _morphSet = new MorphSet(morphs);
        }
    }
}
