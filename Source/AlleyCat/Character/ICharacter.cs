using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Item;
using AlleyCat.Motion;
using AlleyCat.Motion.Generic;
using AlleyCat.Sensor;
using AlleyCat.Sensor.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    public interface ICharacter : IEntity, IMeshObject, IRigged, ILocomotive, ISeeing, IEquipmentHolder
    {
        [CanBeNull]
        IRace Race { get; }

        Sex Sex { get; }
    }

    namespace Generic
    {
        public interface ICharacter<out TVision, out TLocomotion> : ICharacter,
            ISeeing<TVision>, ILocomotive<TLocomotion>
            where TVision : IVision
            where TLocomotion : ILocomotion
        {
        }
    }
}
