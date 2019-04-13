using AlleyCat.Character.Generic;
using AlleyCat.Morph;
using AlleyCat.Motion;
using AlleyCat.Sensor;

namespace AlleyCat.Character
{
    public interface IHumanoid : ICharacter<MorphableRace, IPairedEyeSight, ILocomotion>, IMorphable
    {
    }
}
