using AlleyCat.Action;
using AlleyCat.Character;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public class DropAction : Action.Action
    {
        [Export, UsedImplicitly]
        public string Slot { get; private set; } = "RightHand";

        [Export, UsedImplicitly]
        public string DropPath { get; private set; } = "Items";

        protected override void DoExecute(IActor actor)
        {
            var holder = (IEquipmentHolder) actor;

            var scene = GetTree().CurrentScene;
            var parent = DropPath.TrimToNull() == null ? scene : scene.GetNode(DropPath) ?? scene;

            holder.Unequip(Slot, parent);
        }

        public override bool AllowedFor(IActor context) => context is IHumanoid human && human.HasEquipment(Slot);
    }
}
