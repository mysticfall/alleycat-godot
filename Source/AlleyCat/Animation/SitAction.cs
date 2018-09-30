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

        [Export, UsedImplicitly]
        public Godot.Animation SittingDownAnimation { get; private set; }

        [Export, UsedImplicitly]
        public Godot.Animation Animation { get; private set; }

        [Export, UsedImplicitly]
        public Godot.Animation GettingUpAnimation { get; private set; }

        [Export]
        protected string StatesPath { get; private set; } = "States";

        [Export]
        protected string SubStatesPath { get; private set; } = "States/Seated";

        [Export]
        protected string IdleState { get; private set; } = "Idle";

        [Export]
        protected string EnterState { get; private set; } = "Sitting Down";

        [Export]
        protected string State { get; private set; } = "Seated";

        [Export]
        protected string ExitState { get; private set; } = "Getting Up";

        [Export]
        protected string EnterAnimatorPath { get; private set; } = "States/Seated/Sitting Down";

        [Export]
        protected string AnimatorPath { get; private set; } = "States/Seated/Seated/Sit";

        [Export]
        protected string ExitAnimatorPath { get; private set; } = "States/Seated/Getting Up";

        private bool _valid;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _valid = !string.IsNullOrEmpty(IdleState) &&
                     !string.IsNullOrEmpty(EnterState) &&
                     !string.IsNullOrEmpty(State) &&
                     !string.IsNullOrEmpty(ExitState) &&
                     !string.IsNullOrEmpty(StatesPath) &&
                     !string.IsNullOrEmpty(SubStatesPath) &&
                     !string.IsNullOrEmpty(EnterAnimatorPath) &&
                     !string.IsNullOrEmpty(AnimatorPath) &&
                     !string.IsNullOrEmpty(ExitAnimatorPath);
        }

        protected override void DoExecute(IActionContext context)
        {
            var animatable = (IAnimatable) context.Actor;
            var manager = (IAnimationStateManager) animatable?.AnimationManager;

            if (manager == null) return;

            var states = manager.GetStates(StatesPath);
            var subStates = manager.GetStates(SubStatesPath);

            if (states == null || subStates == null)
            {
                throw new ArgumentException("The specified actor does not support sit action.");
            }

            var enterControl = manager.GetAnimator(EnterAnimatorPath);
            var control = manager.GetAnimator(AnimatorPath);
            var exitControl = manager.GetAnimator(ExitAnimatorPath);

            if (enterControl == null || control == null || exitControl == null)
            {
                throw new ArgumentException("Unable to find suitable controls for sit animations.");
            }

            var current = states.State;

            if (current == IdleState)
            {
                enterControl.Animation = SittingDownAnimation;
                control.Animation = Animation;
                exitControl.Animation = GettingUpAnimation;

                states.State = State;
            }
            else if (current == State && subStates.State == State)
            {
                if (control.Animation == Animation)
                {
                    subStates.State = ExitState;

                    //FIXME A temporary workaround for godotengine/godot#22389
                    states.State = IdleState;
                }
                else
                {
                    enterControl.Animation = SittingDownAnimation;
                    control.Animation = Animation;
                    exitControl.Animation = GettingUpAnimation;
                }
            }
        }

        public SitState GetSitState<T>([NotNull] T actor) where T : class, IActor, IAnimatable
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));

            var animator = actor.AnimationManager as IAnimationStateManager;
            var states = animator?.GetStates(SubStatesPath);

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
            var states = animator?.GetStates(SubStatesPath);

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
