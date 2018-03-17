using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Motion;
using AlleyCat.Motion.Generic;
using AlleyCat.Sensor;
using AlleyCat.Sensor.Generic;
using Godot;

namespace AlleyCat.Character
{
    public interface ICharacter : INamed, IRigged, ILocomotive, ISeeing
    {
        IRace Race { get; }

        Sex Sex { get; }

        Vector3 Viewpoint { get; }

        Vector3 LookingAt { get; }
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
