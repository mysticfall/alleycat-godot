using System;
using System.Diagnostics;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using Godot;
using JetBrains.Annotations;
using static AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion
    {
        [Node]
        public AnimationPlayer AnimationPlayer { get; private set; }

        [Node]
        public AnimationTreePlayer AnimationTreePlayer { get; private set; }

        public RootMotionPlayer RootMotionPlayer => AnimationPlayer as RootMotionPlayer;

        [Export, UsedImplicitly] private NodePath _animationPlayer = "../AnimationPlayer";

        [Export, UsedImplicitly] private NodePath _animationTreePlayer = "../AnimationTreePlayer";

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            AnimationTreePlayer.Active = false;
            AnimationPlayer.PlaybackActive = false;
        }

        protected override Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var momentum = Math.Abs(velocity.x) + Math.Abs(velocity.z);

            if (momentum > 0)
            {
                var ratio = Math.Abs(velocity.z) / momentum;

                AnimationTreePlayer.Blend2NodeSetAmount("Walk", ratio);
            }

            AnimationTreePlayer.Blend3NodeSetAmount("Forward-Backward", -velocity.z);
            AnimationTreePlayer.Blend3NodeSetAmount("Left-Right", velocity.x);

            AnimationTreePlayer.Advance(0);

            RootMotionPlayer.RecordOffset();
            AnimationTreePlayer.Advance(delta);

            RootMotionPlayer.ApplyOffset();

            var offset = RootMotionPlayer.Offset;
            var rotation = new Transform(offset.basis, new Vector3());

            Debug.Assert(Target != null, "Target != null");

            Target.GlobalTransform = rotation * Target.GlobalTransform;
            Target.RotateObjectLocal(Up, rotationalVelocity.y);

            return offset.origin / delta;
        }
    }
}
