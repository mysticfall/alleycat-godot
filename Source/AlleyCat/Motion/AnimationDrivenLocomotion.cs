using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Event;
using AlleyCat.Logging;
using AlleyCat.Setting.Project;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion
    {
        protected AnimationTree AnimationTree { get; }

        protected Skeleton Skeleton { get; }

        protected AnimationStates States { get; }

        protected Blender2D Blender { get; }

        protected string IdleState { get; }

        protected string MoveState { get; }

        public AnimationDrivenLocomotion(
            AnimationTree animationTree,
            Skeleton skeleton,
            AnimationStates states,
            Blender2D blender,
            string idleState,
            string moveState,
            KinematicBody target,
            Physics3DSettings physicsSettings,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(target, physicsSettings, timeSource, active, loggerFactory)
        {
            Ensure.That(animationTree, nameof(animationTree)).IsNotNull();
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();
            Ensure.That(states, nameof(states)).IsNotNull();
            Ensure.That(blender, nameof(blender)).IsNotNull();
            Ensure.That(idleState, nameof(idleState)).IsNotNullOrEmpty();
            Ensure.That(moveState, nameof(moveState)).IsNotNullOrEmpty();

            AnimationTree = animationTree;
            Skeleton = skeleton;
            States = states;
            Blender = blender;
            IdleState = idleState;
            MoveState = moveState;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnActiveStateChange
                .Where(v => !v && Valid)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(_ => ResetAnimations(), this);
        }

        protected override void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            base.Process(delta, velocity, rotationalVelocity);

            Debug.Assert(Target != null, "Target != null");

            var transform = Target.GlobalTransform;
            var basis = transform.basis * Basis.Identity.Rotated(Vector3.Up, -rotationalVelocity.y * delta);

            Target.GlobalTransform = new Transform(basis, transform.origin);
        }

        protected override Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var speed = velocity.Length();
            var direction = velocity.Normalized();

            var current = States.State;

            if (speed > 0)
            {
                if (current == IdleState)
                {
                    States.State = MoveState;
                }

                Blender.Position = new Vector2(direction.x, -direction.z);

                //FIXME Implement proper speed handling here.
                //AnimationTree.TimescaleNodeSetScale(ScaleSpeed, speed);
            }
            else if (current == MoveState)
            {
                States.State = IdleState;
            }

            var t = AnimationTree.GetRootMotionTransform();
            var offset = Skeleton.GlobalTransform.Xform(t.origin) - Skeleton.GlobalTransform.origin;

            return offset / delta;
        }

        protected virtual void ResetAnimations()
        {
            States.State = IdleState;
        }
    }
}
