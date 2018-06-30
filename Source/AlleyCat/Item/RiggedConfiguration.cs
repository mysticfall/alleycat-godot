using Godot;

namespace AlleyCat.Item
{
    public class RiggedConfiguration : EquipmentConfiguration
    {
        public override void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnEquip(holder, equipment);

            foreach (var mesh in equipment.Meshes)
            {
                equipment.Transform = new Transform(Basis.Identity, Vector3.Zero);

                mesh.Skeleton = holder.Skeleton.GetPath();
            }
        }
    }
}
