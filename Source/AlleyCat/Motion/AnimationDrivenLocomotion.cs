using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;
using Gen = System.Collections.Generic;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion
    {
        [Service]
        public IAnimationStateManager AnimationManager { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        protected AnimationStates States { get; private set; }

        protected Blender2D Blender { get; private set; }

        [Export]
        protected string IdleState { get; private set; } = "Idle";

        [Export]
        protected string MoveState { get; private set; } = "Move";

        public override bool Valid => base.Valid &&
                                      IdleState != null &&
                                      MoveState != null &&
                                      States != null &&
                                      Blender != null;

        [Export, UsedImplicitly] private string _statesPath = "States";

        [Export, UsedImplicitly] private string _blend2DPath = "States/Move";

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            States = AnimationManager.GetStates(_statesPath);
            Blender = AnimationManager.GetBlender2D(_blend2DPath);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Subscribe(_ => ResetAnimations())
                .AddTo(this);
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

            var current = States.Playback.GetCurrentNode();

            if (speed > 0)
            {
                if (current != MoveState)
                {
                    States.Playback.Travel(MoveState);
                }

                Blender.Position = new Vector2(direction.x, -direction.z);

                //FIXME Implement proper speed handling here.
                //AnimationTree.TimescaleNodeSetScale(ScaleSpeed, speed);
            }
            else if (current == MoveState)
            {
                States.Playback.Travel(IdleState);
            }

            var t = AnimationManager.AnimationTree.GetRootMotionTransform();
            var offset = Skeleton.GlobalTransform.Xform(t.origin) - Skeleton.GlobalTransform.origin;

            return offset / delta;
        }

        protected virtual void ResetAnimations()
        {
            States.Playback.Travel(IdleState);
        }
    }
}
