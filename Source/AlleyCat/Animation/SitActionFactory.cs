using AlleyCat.Action;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class SitActionFactory : ActionFactory<SitAction>
    {
        [Export]
        public Godot.Animation SittingDownAnimation { get; set; }

        [Export]
        public Godot.Animation Animation { get; set; }

        [Export]
        public Godot.Animation GettingUpAnimation { get; set; }

        [Export]
        public string StatesPath { get; set; } = "States";

        [Export]
        public string SubStatesPath { get; set; } = "States/Seated";

        [Export]
        public string IdleState { get; set; } = "Idle";

        [Export]
        public string EnterState { get; set; } = "Sitting Down";

        [Export]
        public string State { get; set; } = "Seated";

        [Export]
        public string ExitState { get; set; } = "Getting Up";

        [Export]
        public string EnterAnimatorPath { get; set; } = "States/Seated/Sitting Down";

        [Export]
        public string AnimatorPath { get; set; } = "States/Seated/Seated/Sit";

        [Export]
        public string ExitAnimatorPath { get; set; } = "States/Seated/Getting Up";

        protected override Validation<string, SitAction> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return
                from animation in Optional(Animation)
                    .ToValidation("Animation was not specified.")
                from statesPath in StatesPath.TrimToOption()
                    .ToValidation("States path was not specified.")
                from subStatesPath in SubStatesPath.TrimToOption()
                    .ToValidation("Sub-states path was not specified.")
                from idleState in IdleState.TrimToOption()
                    .ToValidation("Idle state value was not specified.")
                from enterState in EnterState.TrimToOption()
                    .ToValidation("Enter state value was not specified.")
                from state in State.TrimToOption()
                    .ToValidation("State path was not specified.")
                from exitState in ExitState.TrimToOption()
                    .ToValidation("Exit state value was not specified.")
                from enterAnimatorPath in EnterAnimatorPath.TrimToOption()
                    .ToValidation("Enter animator path was not specified.")
                from animatorPath in AnimatorPath.TrimToOption()
                    .ToValidation("Animator path was not specified.")
                from exitAnimatorPath in ExitAnimatorPath.TrimToOption()
                    .ToValidation("Exit animator path was not specified.")
                select new SitAction(
                    key,
                    displayName,
                    animation,
                    statesPath,
                    subStatesPath,
                    idleState,
                    enterState,
                    state,
                    exitState,
                    enterAnimatorPath,
                    animatorPath,
                    exitAnimatorPath,
                    Active,
                    loggerFactory)
                {
                    SittingDownAnimation = SittingDownAnimation,
                    GettingUpAnimation = GettingUpAnimation
                };
        }
    }
}
