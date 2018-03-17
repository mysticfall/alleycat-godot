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
        public string Key => _key ?? Name;

        public virtual string DisplayName => _displayName;

        public virtual IRace Race => RaceRegistry?[_race];

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

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        [Export, UsedImplicitly] private string _race;

        [Export, UsedImplicitly] private Sex _sex;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
