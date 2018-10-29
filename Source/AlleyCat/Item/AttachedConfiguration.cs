using Godot;

namespace AlleyCat.Item
{
    public class AttachedConfiguration : EquipmentConfiguration
    {
        public override void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnEquip(holder, equipment);

            var transform = equipment.Markers.Find(Key).Map(m => m.Transform.Inverse())
                .IfNone(() => new Transform(Basis.Identity, Vector3.Zero));

            foreach (var mesh in equipment.Meshes)
            {
                equipment.Transform = transform;

                mesh.Skeleton = mesh.GetPathTo(equipment);
            }
        }
    }
}
