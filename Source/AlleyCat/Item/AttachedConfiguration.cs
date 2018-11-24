using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class AttachedConfiguration : EquipmentConfiguration
    {
        public AttachedConfiguration(
            string key,
            string slot,
            Set<string> additionalSlots,
            Set<string> tags,
            bool active,
            ILoggerFactory loggerFactory) : base(key, slot, additionalSlots, tags, active, loggerFactory)
        {
        }

        public override void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnEquip(holder, equipment);

            var transform = equipment.Markers.Find(Key).Map(m => m.Transform.Inverse())
                .IfNone(() => new Transform(Basis.Identity, Vector3.Zero));

            foreach (var mesh in equipment.Meshes)
            {
                equipment.SetTransform(transform);

                mesh.Skeleton = mesh.GetPathTo(equipment.Node);
            }
        }
    }
}
