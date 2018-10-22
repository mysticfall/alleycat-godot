using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static AlleyCat.Animation.SitState;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class SitAction : Action.Action
    {
        public override bool Valid => base.Valid && _valid;

        public Option<Godot.Animation> SittingDownAnimation
        {
            get => Optional(_sittingDownAnimation);
            set => _sittingDownAnimation = value.ValueUnsafe();
        }

        public Godot.Animation Animation
        {
            get => Some(_animation).Head();
            set
            {
                Ensure.That(value, nameof(value)).IsNotNull();

                _animation = value;
            }
        }

        public Option<Godot.Animation> GettingUpAnimation
        {
            get => Optional(_gettingUpAnimation);
            set => _gettingUpAnimation = value.ValueUnsafe();
        }

        protected string StatesPath => _statesPath.TrimToOption().Head();

        protected string SubStatesPath => _subStatesPath.TrimToOption().Head();

        protected string IdleState => _idleState.TrimToOption().Head();

        protected string EnterState => _enterState.TrimToOption().Head();

        protected string State => _state.TrimToOption().Head();

        protected string ExitState => _exitState.TrimToOption().Head();

        protected string EnterAnimatorPath => _enterAnimatorPath.TrimToOption().Head();

        protected string AnimatorPath => _animatorPath.TrimToOption().Head();

        protected string ExitAnimatorPath => _exitAnimatorPath.TrimToOption().Head();

        [Export, UsedImplicitly] private Godot.Animation _sittingDownAnimation;

        [Export, UsedImplicitly] private Godot.Animation _animation;

        [Export, UsedImplicitly] private Godot.Animation _gettingUpAnimation;

        [Export, UsedImplicitly] private string _statesPath = "States";

        [Export, UsedImplicitly] private string _subStatesPath = "States/Seated";

        [Export, UsedImplicitly] private string _idleState = "Idle";

        [Export, UsedImplicitly] private string _enterState = "Sitting Down";

        [Export, UsedImplicitly] private string _state = "Seated";

        [Export, UsedImplicitly] private string _exitState = "Getting Up";

        [Export, UsedImplicitly] private string _enterAnimatorPath = "States/Seated/Sitting Down";

        [Export, UsedImplicitly] private string _animatorPath = "States/Seated/Seated/Sit";

        [Export, UsedImplicitly] private string _exitAnimatorPath = "States/Seated/Getting Up";

        private bool _valid;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _valid = StatesPath != null &&
                     SubStatesPath != null &&
                     IdleState != null &&
                     EnterState != null &&
                     State != null &&
                     ExitState != null &&
                     EnterAnimatorPath != null &&
                     AnimatorPath != null &&
                     ExitAnimatorPath != null;
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            Debug.Assert(Valid, "Valid");

            var manager = context.Actor.Bind(GetAnimationStateManager);

            var states = manager.Bind(m => m.FindStates(StatesPath));
            var subStates = manager.Bind(m => m.FindStates(SubStatesPath));

            if (states.IsNone || subStates.IsNone)
            {
                throw new ArgumentException("The specified actor does not support sit action.");
            }

            var enterControl = manager.Bind(m => m.FindAnimator(EnterAnimatorPath));
            var control = manager.Bind(m => m.FindAnimator(AnimatorPath));
            var exitControl = manager.Bind(m => m.FindAnimator(ExitAnimatorPath));

            if (enterControl.IsNone || control.IsNone || exitControl.IsNone)
            {
                throw new ArgumentException("Unable to find suitable controls for sit animations.");
            }

            var current = states.Map(s => s.State);

            if (current.Contains(IdleState))
            {
                UpdateAnimations();

                states.Iter(s => s.State = State);
            }
            else if (current.Contains(State) && subStates.Exists(s => s.State == State))
            {
                if (control.Exists(c => c.Animation.Contains(Animation)))
                {
                    subStates.Iter(s => s.State = ExitState);

                    //FIXME A temporary workaround for godotengine/godot#22389
                    states.Iter(s => s.State = IdleState);
                }
                else
                {
                    UpdateAnimations();
                }
            }

            void UpdateAnimations()
            {
                enterControl.Iter(c => c.Animation = SittingDownAnimation);
                control.Iter(c => c.Animation = Animation);
                exitControl.Iter(c => c.Animation = GettingUpAnimation);
            }
        }

        private Option<IAnimationStateManager> GetAnimationStateManager(IActor actor)
        {
            Debug.Assert(actor != null, "actor != null");

            return Optional(actor)
                .OfType<IAnimatable>()
                .Map(a => a.AnimationManager)
                .OfType<IAnimationStateManager>()
                .HeadOrNone();
        }

        public SitState GetSitState<T>(T actor) where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var animator = GetAnimationStateManager(actor);
            var state = animator.Bind(a => a.FindStates(SubStatesPath)).Map(s => s.State);

            return state.Map(GetStateEnum).Match(v => v,
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.")
            );
        }

        public IObservable<SitState> OnSitStateChange<T>(T actor)
            where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var animator = GetAnimationStateManager(actor);
            var states = animator
                .Bind(a => a.FindStates(SubStatesPath))
                .Map(s => s.OnStateChange.Select(GetStateEnum));

            return states.Match(identity,
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.")
            );
        }

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context.Actor.Bind(GetAnimationStateManager).IsSome;
        }

        private SitState GetStateEnum(string state)
        {
            if (state == EnterState) return SittingDown;
            if (state == State) return Seated;
            if (state == ExitState) return GettingUp;

            return Standing;
        }
    }

    public static class SitExtensions
    {
        public static SitState GetSitState<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var action = actor.Actions.Values.OfType<SitAction>().HeadOrNone();

            return action.Map(a => a.GetSitState(actor)).Match(v => v,
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action."));
        }

        public static IObservable<SitState> OnSitStateChange<T>(this T actor)
            where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var action = actor.Actions.Values.OfType<SitAction>().HeadOrNone();

            return action.Map(a => a.OnSitStateChange(actor)).Match(v => v,
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action."));
        }

        public static void ToggleSitState<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var action = actor.Actions.Values.OfType<SitAction>().HeadOrNone();

            action.Match(
                a => a.Execute(new ActionContext(actor)),
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action."));
        }

        public static void Sit<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var action = actor.Actions.Values.OfType<SitAction>().HeadOrNone();

            if (action.IsNone)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            var state = action.Map(a => a.GetSitState(actor));

            (from a in action
                from s in state
                where s == Standing || s == GettingUp
                select a).Iter(a => a.Execute(new ActionContext(actor)));
        }

        public static void GetUp<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var action = actor.Actions.Values.OfType<SitAction>().HeadOrNone();

            if (action.IsNone)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            var state = action.Map(a => a.GetSitState(actor));

            (from a in action
                from s in state
                where s == Seated || s == SittingDown
                select a).Iter(a => a.Execute(new ActionContext(actor)));
        }
    }
}
