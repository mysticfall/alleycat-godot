using System;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Action
{
    public class GetUpAction : Action
    {
        protected string StatesPath { get; }

        protected string SubStatesPath { get; }

        protected string State { get; }

        protected string ExitState { get; }

        public GetUpAction(
            string key,
            string displayName,
            string statesPath,
            string subStatesPath,
            string state,
            string exitState,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, active, loggerFactory)
        {
            Ensure.That(statesPath, nameof(statesPath)).IsNotNull();
            Ensure.That(subStatesPath, nameof(subStatesPath)).IsNotNull();
            Ensure.That(state, nameof(state)).IsNotNull();
            Ensure.That(exitState, nameof(exitState)).IsNotNull();

            StatesPath = statesPath;
            SubStatesPath = subStatesPath;
            State = state;
            ExitState = exitState;
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

            var current = states.Map(s => s.State);

            if (current.Contains(State))
            {
                this.LogDebug("Getting up");

                subStates.Iter(s => s.State = ExitState);
            }
            else
            {
                this.LogDebug("Ignoring sit state '{}'", current);
            }
        }

        private Option<IAnimationStateManager> GetAnimationStateManager(IActor actor)
        {
            Debug.Assert(actor != null, "actor != null");

            return Prelude.Optional(actor)
                .OfType<IAnimatable>()
                .Map(a => a.AnimationManager)
                .OfType<IAnimationStateManager>()
                .HeadOrNone();
        }

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            var animator = context.Actor.Bind(GetAnimationStateManager);
            var state = animator.Bind(a => a.FindStates(SubStatesPath)).Map(s => s.State);

            return state.Contains(State);
        }
    }

    public static class GetUpExtensions
    {
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
                where s == SitState.Seated || s == SitState.SittingDown
                select a).Iter(a => a.Execute(new ActionContext(actor)));
        }
    }
}
