using AlleyCat.Action;
using AlleyCat.Game;
using AlleyCat.Item;
using AlleyCat.Motion;
using AlleyCat.Motion.Generic;
using AlleyCat.Sensor;
using AlleyCat.Sensor.Generic;

namespace AlleyCat.Character
{
    public interface ICharacter : IEntity, IActor, ILocomotive, ISeeing, IEquipmentHolder
    {
        Race Race { get; }

        Sex Sex { get; }
    }

    namespace Generic
    {
        public interface ICharacter<out TRace, out TVision, out TLocomotion> :
            ICharacter, ISeeing<TVision>, ILocomotive<TLocomotion>
            where TRace : Race
            where TVision : IVision
            where TLocomotion : ILocomotion
        {
            new TRace Race { get; }
        }
    }
}
