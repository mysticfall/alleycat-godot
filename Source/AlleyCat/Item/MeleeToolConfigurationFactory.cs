using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Logging;
using static Godot.Mathf;

namespace AlleyCat.Item
{
    public class MeleeToolConfigurationFactory : EquipmentConfigurationFactory<MeleeToolConfiguration>
    {
        [Export]
        public Godot.Animation SwingAnimation { get; set; }

        [Service]
        public Option<IPlayerControl> PlayerControl { get; set; }

        [Node("Control/Arm")]
        public Option<IInputBindings> ArmInput { get; set; }

        [Node("Control/Swing")]
        public Option<IInputBindings> SwingInput { get; set; }

        [Export]
        public string IdleState { get; set; } = "Idle";

        [Export]
        public string SwingState { get; set; } = "Swing";

        [Export]
        public string StatesPath { get; set; } = "States";

        [Export]
        public string SeekerPath { get; set; } = "States/Swing/Swing";

        [Export]
        public float MaxPosition { get; set; }

        [Export]
        public float MinPosition { get; set; }

        protected override Validation<string, MeleeToolConfiguration> CreateService(
            string key,
            string slot,
            Set<string> additionalSlots,
            Set<string> tags,
            ILoggerFactory loggerFactory)
        {
            return
                from swingAnimation in Optional(SwingAnimation)
                    .ToValidation("Missing swing animation.")
                from statesPath in StatesPath.TrimToOption()
                    .ToValidation("Missing animation state machine path.")
                from seekerPath in SeekerPath.TrimToOption()
                    .ToValidation("Missing seekable animator path.")
                from playerControl in PlayerControl
                    .ToValidation("Failed to find the player control.")
                from idleState in IdleState.TrimToOption()
                    .ToValidation("Idle state value was not specified.")
                from swingState in SwingState.TrimToOption()
                    .ToValidation("Swing state value was not specified.")
                select new MeleeToolConfiguration(
                    key,
                    slot,
                    additionalSlots,
                    tags,
                    ArmInput,
                    SwingInput,
                    swingAnimation,
                    statesPath,
                    seekerPath,
                    idleState,
                    swingState,
                    new Range<float>(Max(MinPosition, 0), Max(MaxPosition, 0), TFloat.Inst),
                    playerControl,
                    Active,
                    loggerFactory)
                {
                    Mesh = Mesh,
                    EquipAnimation = EquipAnimation,
                    UnequipAnimation = UnequipAnimation,
                    Animation = Animation,
                    AnimationBlend = AnimationBlend,
                    AnimationTransition = AnimationTransition
                };
        }
    }
}
