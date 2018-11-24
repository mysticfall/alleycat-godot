using AlleyCat.Action;
using AlleyCat.Common;
using Godot;
using Godot.Collections;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class PickupActionFactory : ActionFactory<PickupAction>
    {
        [Export(PropertyHint.ExpRange, "0.1, 5")]
        public float PickupDistance { get; set; } = 1.2f;

        [Export]
        public Godot.Animation Animation { get; set; }

        [Export]
        public string AnimatorPath { get; set; } = "States/Action";

        [Export]
        public string StatesPath { get; set; } = "States";

        [Export]
        public string ActionState { get; set; } = "Action";

        [Export]
        public string IkChain { get; set; } = "Right Hand IK";

        [Export]
        public Array<string> Tags { get; set; } = new Array<string> {Carry, Hand};

        protected override Validation<string, PickupAction> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return new PickupAction(
                key,
                displayName,
                toSet(Tags),
                IkChain.TrimToOption(),
                AnimatorPath.TrimToOption(),
                StatesPath.TrimToOption(),
                ActionState.TrimToOption(),
                Active,
                loggerFactory)
            {
                PickupDistance = PickupDistance,
                Animation = Animation
            };
        }
    }
}
