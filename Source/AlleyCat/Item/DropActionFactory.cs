using AlleyCat.Action;
using AlleyCat.Common;
using static AlleyCat.Item.CommonEquipmentTags;
using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
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

        [Export]
        public Array<string> Tags { get; set; } = new Array<string>(new[] {Carry});

        protected override Validation<string, DropAction> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return new DropAction(
                key,
                displayName,
                AnimatorPath.TrimToOption(),
                StatesPath.TrimToOption(),
                ActionState.TrimToOption(),
                toSet(Optional(Tags).Flatten()),
                Active,
                loggerFactory)
            {
                Animation = Animation
            };
        }
    }
}
