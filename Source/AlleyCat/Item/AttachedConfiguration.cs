using Godot;

namespace AlleyCat.Item
{
    public class AttachedConfiguration : EquipmentConfiguration
    {
        public override void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnEquip(holder, equipment);

            var trannsform = equipment.Markers.TryGetValue(Key, out var attachPoint)
                ? attachPoint.Transform.Inverse()
                : new Transform(Basis.Identity, Vector3.Zero);

            foreach (var mesh in equipment.Meshes)
            {
                equipment.Transform = trannsform;

                mesh.Skeleton = equipment.GetPath();
            }
        }
    }
}
