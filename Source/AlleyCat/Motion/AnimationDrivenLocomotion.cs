using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using Gen = System.Collections.Generic;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion
    {
        [Service]
        public IAnimationStateManager AnimationManager { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        [Export]
        public string IdleState { get; set; } = "Idle";

        [Export]
        public string MoveState { get; set; } = "Move";

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Debug.Assert(IdleState != null, "IdleState != null");
            Debug.Assert(MoveState != null, "MoveState != null");

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Subscribe(_ => ResetAnimations())
                .AddTo(this);

            AnimationManager.States.Start(IdleState);
        }

        protected override void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            base.Process(delta, velocity, rotationalVelocity);

            Debug.Assert(Target != null, "Target != null");

            var transform = Target.GlobalTransform;
            var basis = transform.basis *
                        Basis.Identity.Rotated(Vector3.Up, rotationalVelocity.y * delta);

            Target.GlobalTransform = new Transform(basis, transform.origin);
        }

        protected override Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var speed = velocity.Length();
            var direction = velocity.Normalized();

            var states = AnimationManager.States;

            if (speed > 0)
            {
                if (states.GetCurrentNode() != MoveState)
                {
                    states.Travel(MoveState);
                }

                var blender = (AnimationNodeBlendSpace2D) states.GetNode(MoveState);

                blender.SetBlendPosition(new Vector2(direction.x, -direction.z));

                //FIXME Implement proper speed handling here.
                //AnimationTree.TimescaleNodeSetScale(ScaleSpeed, speed);
            }
            else
            {
                states.Travel(IdleState);
            }

            var t = AnimationManager.AnimationTree.GetRootMotionTransform();
            var offset = Skeleton.GlobalTransform.Xform(t.origin) - Skeleton.GlobalTransform.origin;

            return offset / delta;
        }

        protected virtual void ResetAnimations()
        {
            AnimationManager.States.Travel(IdleState);
        }
    }
}
