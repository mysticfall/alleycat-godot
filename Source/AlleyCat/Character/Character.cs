using AlleyCat.Common;
using AlleyCat.Locomotion;
using Godot;

namespace AlleyCat.Character
{
    public class Character : KinematicBody, ICharacter
    {
        public ILocomotion Locomotion { get; private set; }

        public AnimationPlayer AnimationPlayer { get; private set; }

        public Skeleton Skeleton { get; private set; }

        [Export] private NodePath _skeletonPath = "Skeleton";

        [Export] private NodePath _animationPlayerPath = "AnimationPlayer";

        [Export] private NodePath _locomotionPath = "Locomotion";

        public override void _Ready()
        {
            base._Ready();

            Locomotion = this.GetNode<ILocomotion>(_locomotionPath);
            AnimationPlayer = this.GetNode<AnimationPlayer>(_animationPlayerPath);
            Skeleton = this.GetNode<Skeleton>(_skeletonPath);
        }
    }
}
