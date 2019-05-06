using AlleyCat.Action;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Animation
{
    public class GetUpActionFactory : ActionFactory<GetUpAction>
    {
        [Export]
        public string StatesPath { get; set; } = "States";

        [Export]
        public string SubStatesPath { get; set; } = "States/Seated";

        [Export]
        public string State { get; set; } = "Seated";

        [Export]
        public string ExitState { get; set; } = "Getting Up";

        protected override Validation<string, GetUpAction> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return
                from statesPath in StatesPath.TrimToOption()
                    .ToValidation("States path was not specified.")
                from subStatesPath in SubStatesPath.TrimToOption()
                    .ToValidation("Sub-states path was not specified.")
                from state in State.TrimToOption()
                    .ToValidation("State path was not specified.")
                from exitState in ExitState.TrimToOption()
                    .ToValidation("Exit state value was not specified.")
                select new GetUpAction(
                    key,
                    displayName,
                    statesPath,
                    subStatesPath,
                    state,
                    exitState,
                    Active,
                    loggerFactory);
        }
    }
}
