using AlleyCat.Autowire;
using AlleyCat.Character.Generic;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class Character<TVision, TLocomotion> : KinematicBody, ICharacter<TVision, TLocomotion>
        where TVision : class, IVision
        where TLocomotion : class, ILocomotion
    {
        public string Key => _key;

        [Export, UsedImplicitly]
        public virtual string DisplayName { get; private set; }

        public virtual IRace Race => RaceRegistry?[_race];

        [Export, UsedImplicitly]
        public virtual Sex Sex => _sex;

        [Service]
        public TVision Vision { get; private set; }

        [Service]
        public TLocomotion Locomotion { get; private set; }

        [Service]
        public AnimationPlayer AnimationPlayer { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        [Service]
        protected IRaceRegistry RaceRegistry { get; private set; }

        public Vector3 Viewpoint => Vision.Origin;

        public Vector3 LookingAt => Vision.Forward;

        [Export, UsedImplicitly] private string _race;

        [Export, UsedImplicitly] private Sex _sex;

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;

        [Export, UsedImplicitly]
        private string _key;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
