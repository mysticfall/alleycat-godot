using AlleyCat.Action;
using AlleyCat.Character;
using AlleyCat.Game;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class DropAction : EquipmentAction
    {
        protected override void DoExecute(IActor actor)
        {
            var holder = (IEquipmentHolder) actor;

            var scene = GetTree().CurrentScene;
            var path = ((IScene) scene).ItemsPath;

            var parent = scene.GetNode(path) ?? scene;

            holder.Unequip(Item, parent);
        }

        public override bool AllowedFor(IActor context) =>
            Item.Slot != null &&
            context is IHumanoid human &&
            human.HasEquipment(Item.Slot) &&
            Item.Configuration.HasTag(Carry);
    }
}
