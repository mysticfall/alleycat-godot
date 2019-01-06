using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static AlleyCat.Animation.SitState;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class SitAction : Action.Action
    {
        public Option<Godot.Animation> SittingDownAnimation { get; set; }

        public Godot.Animation Animation
        {
            get => Some(_animation).Head();
            set
            {
                Ensure.That(value, nameof(value)).IsNotNull();

                _animation = value;
            }
        }

        public Option<Godot.Animation> GettingUpAnimation { get; set; }

        public float Transition
        {
            get => _transition;
            set => _transition = Mathf.Max(0, value);
        }

        protected string StatesPath { get; }

        protected string SubStatesPath { get; }

        protected string IdleState { get; }

        protected string EnterState { get; }

        protected string State { get; }

        protected string ExitState { get; }

        protected string EnterAnimatorPath { get; }

        protected string AnimatorPath { get; }

        protected string ExitAnimatorPath { get; }

        private Godot.Animation _animation;

        private float _transition = 0.5f;

        public SitAction(
            string key,
            string displayName,
            Godot.Animation animation,
            string statesPath,
            string subStatesPath,
            string idleState,
            string enterState,
            string state,
            string exitState,
            string enterAnimatorPath,
            string animatorPath,
            string exitAnimatorPath,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, active, loggerFactory)
        {
            Ensure.That(animation, nameof(animation)).IsNotNull();
            Ensure.That(statesPath, nameof(statesPath)).IsNotNull();
            Ensure.That(subStatesPath, nameof(subStatesPath)).IsNotNull();
            Ensure.That(idleState, nameof(idleState)).IsNotNull();
            Ensure.That(enterState, nameof(enterState)).IsNotNull();
            Ensure.That(state, nameof(state)).IsNotNull();
            Ensure.That(exitState, nameof(exitState)).IsNotNull();
            Ensure.That(enterAnimatorPath, nameof(enterAnimatorPath)).IsNotNull();
            Ensure.That(animatorPath, nameof(animatorPath)).IsNotNull();
            Ensure.That(exitAnimatorPath, nameof(exitAnimatorPath)).IsNotNull();

            Animation = animation;

            StatesPath = statesPath;
            SubStatesPath = subStatesPath;
            IdleState = idleState;
            EnterState = enterState;
            State = state;
            ExitState = exitState;
            EnterAnimatorPath = enterAnimatorPath;
            AnimatorPath = animatorPath;
            ExitAnimatorPath = exitAnimatorPath;
        }

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

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

                this.LogDebug("Sitting down");

                states.Iter(s => s.State = State);
            }
            else if (current.Contains(State))
            {
                if (control.Exists(c => c.Animation.Contains(Animation)))
                {
                    this.LogDebug("Getting up");

                    subStates.Iter(s => s.State = ExitState);
                }
                else
                {
                    this.LogDebug("Changing sitting posture");

                    UpdateAnimations(Transition);
                }
            }
            else
            {
                this.LogDebug("Ignoring sit state '{}'", current);
            }

            void UpdateAnimations(float transition = 0)
            {
                enterControl.Iter(c => c.Animation = SittingDownAnimation);
                control.OfType<CrossfadingAnimator>().Iter(c => c.Time = transition);
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
