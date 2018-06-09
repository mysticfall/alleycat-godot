using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Item
{
    public class PickupAction : Action.Action
    {
        [Ancestor]
        public IItem Item { get; private set; }

        [Export(PropertyHint.ExpRange, "0.1, 5")]
        public float PickupDistance { get; set; } = 1f;

        [Export]
        public string Animation { get; set; } = "Pickup";

        protected override void DoExecute(IActor actor)
        {
            var animator = (actor as ICharacter)?.AnimationManager as IAnimationStateManager;

            animator?.TreePlayer.OneshotNodeStart(Animation);
        }

        public override bool AllowedFor(IActor context) =>
            context is ICharacter character && character.DistanceTo(Item) <= PickupDistance;
    }
}
