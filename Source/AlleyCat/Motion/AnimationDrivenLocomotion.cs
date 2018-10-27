using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;

namespace AlleyCat.Motion
{
    public class AnimationDrivenLocomotion : KinematicLocomotion
    {
        public IAnimationStateManager AnimationManager => _animationManager.Head();

        public Skeleton Skeleton => _skeleton.Head();

        protected AnimationStates States => _states.Head();

        protected Blender2D Blender => _blender.Head();

        protected string IdleState => _idleState.TrimToOption().Head();

        protected string MoveState => _moveState.TrimToOption().Head();

        public override bool Valid => base.Valid && _valid;

        [Service] private Option<IAnimationStateManager> _animationManager;

        [Service] private Option<Skeleton> _skeleton;

        [Export] private string _idleState = "Idle";

        [Export] private string _moveState = "Moving";

        [Export] private string _statesPath = "States";

        [Export] private string _blend2DPath = "States/Moving";

        private Option<AnimationStates> _states;

        private Option<Blender2D> _blender;

        private bool _valid;

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            _states = _statesPath.TrimToOption().Bind(AnimationManager.FindStates);
            _blender = _blend2DPath.TrimToOption().Bind(AnimationManager.FindBlender2D);

            OnActiveStateChange
                .Where(v => !v && Valid)
                .Subscribe(_ => ResetAnimations())
                .AddTo(this);

            _valid = _states.IsSome &&
                     _blender.IsSome &&
                     !string.IsNullOrWhiteSpace(_idleState) &&
                     !string.IsNullOrWhiteSpace(_moveState);
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

            var t = AnimationManager.AnimationTree.GetRootMotionTransform();
            var offset = Skeleton.GlobalTransform.Xform(t.origin) - Skeleton.GlobalTransform.origin;

            return offset / delta;
        }

        protected virtual void ResetAnimations()
        {
            States.State = IdleState;
        }
    }
}
