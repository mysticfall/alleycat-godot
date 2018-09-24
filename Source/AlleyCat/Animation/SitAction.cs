using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using static AlleyCat.Animation.SitState;

namespace AlleyCat.Animation
{
    public class SitAction : Action.Action
    {
        public override bool Valid => base.Valid && _valid;

        [Export]
        protected string IdleState { get; private set; } = "Idle";

        [Export]
        protected string SitState { get; private set; } = "Sit";

        [Export]
        protected string SitStartState { get; private set; } = "Sit Start";

        [Export]
        protected string SitEndState { get; private set; } = "Sit End";

        [Export]
        protected string StatesPath { get; private set; } = "States";

        [Export]
        protected string SitStatesPath { get; private set; } = "States/Sit";

        private bool _valid;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _valid = !string.IsNullOrEmpty(IdleState) &&
                     !string.IsNullOrEmpty(SitState) &&
                     !string.IsNullOrEmpty(SitStartState) &&
                     !string.IsNullOrEmpty(SitEndState) &&
                     !string.IsNullOrEmpty(StatesPath) &&
                     !string.IsNullOrEmpty(SitStatesPath);
        }

        protected override void DoExecute(IActionContext context)
        {
            var animatable = (IAnimatable) context.Actor;
            var animator = (IAnimationStateManager) animatable?.AnimationManager;

            if (animatable == null) return;

            var states = animator.GetStates(StatesPath);
            var sitStates = animator.GetStates(SitStatesPath);

            if (states == null || sitStates == null)
            {
                throw new ArgumentException("The specified actor does not support sit action.");
            }

            var state = GetStateEnum(states.State);

            switch (state)
            {
                case Seated:
                case SittingDown:
                    sitStates.State = SitEndState;

                    //FIXME A temporary workaround for godotengine/godot#22389
                    states.State = IdleState;
                    break;
                case Standing:
                    states.State = SitState;
                    break;
                case GettingUp:
                    sitStates.State = SitStartState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(context),
                        $"The actor is in an unknown state: '{state}'.");
            }
        }

        public SitState GetSitState<T>([NotNull] T actor) where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var animator = actor.AnimationManager as IAnimationStateManager;
            var states = animator?.GetStates(SitStatesPath);

            if (states == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            return GetStateEnum(states.State);
        }

        public IObservable<SitState> OnSitStateChange<T>([NotNull] T actor)
            where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var animator = actor.AnimationManager as IAnimationStateManager;
            var states = animator?.GetStates(SitStatesPath);

            if (states == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            return states.OnStateChange.Select(GetStateEnum);
        }

        public override bool AllowedFor(IActionContext context) =>
            context?.Actor is IAnimatable animatable &&
            animatable.AnimationManager is IAnimationStateManager;

        private SitState GetStateEnum(string state)
        {
            if (state == SitStartState) return SittingDown;
            if (state == SitState) return Seated;
            if (state == SitEndState) return GettingUp;

            return Standing;
        }
    }

    public static class SitExtensions
    {
        public static SitState GetSitState<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var action = actor.Actions.Values.FirstOrDefault(a => a is SitAction);

            if (action == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            return ((SitAction) action).GetSitState(actor);
        }

        public static IObservable<SitState> OnSitStateChange<T>(this T actor)
            where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var action = actor.Actions.Values.FirstOrDefault(a => a is SitAction);

            if (action == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            return ((SitAction) action).OnSitStateChange(actor);
        }

        public static void ToggleSitState<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var action = actor.Actions.Values.FirstOrDefault(a => a is SitAction);

            if (action == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            action.Execute(new ActionContext(actor));
        }

        public static void Sit<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var action = actor.Actions.Values.FirstOrDefault(a => a is SitAction);

            if (action == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            var state = ((SitAction) action).GetSitState(actor);

            if (state == Standing || state == GettingUp)
            {
                action.Execute(new ActionContext(actor));
            }
        }

        public static void GetUp<T>(this T actor) where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var action = actor.Actions.Values.FirstOrDefault(a => a is SitAction);

            if (action == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actor),
                    "The specified actor does not support sit action.");
            }

            var state = ((SitAction) action).GetSitState(actor);

            if (state == Seated || state == SittingDown)
            {
                action.Execute(new ActionContext(actor));
            }
        }
    }
}
