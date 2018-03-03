using AlleyCat.Autowire;
using AlleyCat.Motion;
using Godot;

namespace AlleyCat.Character
{
    [AutowireContext]
    public class Character : KinematicBody, ICharacter
    {
        [Service]
        public ILocomotion Locomotion { get; private set; }

        [Service]
        public AnimationPlayer AnimationPlayer { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
