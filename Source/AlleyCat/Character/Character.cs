using AlleyCat.Autowire;
using AlleyCat.Character.Generic;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class Character<TVision, TLocomotion> : KinematicBody, ICharacter<TVision, TLocomotion>
        where TVision : class, IVision
        where TLocomotion : class, ILocomotion
    {
        [Service]
        public TVision Vision { get; private set; }

        [Service]
        public TLocomotion Locomotion { get; private set; }

        [Service]
        public AnimationPlayer AnimationPlayer { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        public Vector3 Viewpoint => Vision.Origin;

        public Vector3 LookingAt => Vision.Forward;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;
    }
}
