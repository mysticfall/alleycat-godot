using AlleyCat.Character.Generic;
using AlleyCat.Motion;
using AlleyCat.Sensor;

namespace AlleyCat.Character
{
    public interface IHumanoid : ICharacter<IPairedEyeSight, ILocomotion>
    {
    }
}
