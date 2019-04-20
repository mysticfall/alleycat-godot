using AlleyCat.Action;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class DropActionFactory : ActionFactory<DropAction>
    {
        [Export]
        public Godot.Animation Animation { get; set; }

        [Export]
        public string AnimatorPath { get; set; } = "States/Action";

        [Export]
        public string StatesPath { get; set; } = "States";

        [Export]
        public string ActionState { get; set; } = "Action";

        protected override Validation<string, DropAction> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return new DropAction(
                key,
                displayName,
                AnimatorPath.TrimToOption(),
                StatesPath.TrimToOption(),
                ActionState.TrimToOption(),
                Active,
                loggerFactory)
            {
                Animation = Animation
            };
        }
    }
}
