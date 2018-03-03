using AlleyCat.Autowire;
using AlleyCat.Motion;
using Godot;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class Character : KinematicBody, ICharacter
    {
        [Service]
        public ILocomotion Locomotion { get; private set; }

        [Service]
        public AnimationPlayer AnimationPlayer { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        public abstract Vector3 Viewpoint { get; }

        public abstract Vector3 LookingAt { get; }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
