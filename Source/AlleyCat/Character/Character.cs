using AlleyCat.Autowire;
using AlleyCat.Locomotion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    [AutowireContext]
    public class Character : KinematicBody, ICharacter
    {
        [Service]
        public ILocomotion Locomotion { get; private set; }

        [Node]
        public AnimationPlayer AnimationPlayer { get; private set; }

        [Node]
        public Skeleton Skeleton { get; private set; }

        [Export, UsedImplicitly] private NodePath _skeleton = "Skeleton";

        [Export, UsedImplicitly] private NodePath _animationPlayer = "AnimationPlayer";

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
