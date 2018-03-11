using AlleyCat.Animation;
using AlleyCat.Motion;
using AlleyCat.Motion.Generic;
using AlleyCat.Sensor;
using AlleyCat.Sensor.Generic;
using Godot;

namespace AlleyCat.Character
{
    public interface ICharacter : IRigged, ILocomotive, ISeeing
    {
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
