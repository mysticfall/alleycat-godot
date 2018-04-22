using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public class RiggedCloth : SlotItem, IEquippable
    {
        [Export, UsedImplicitly]
        public Mesh EquippedMesh { get; private set; }

        public override bool AllowedFor(ISlotContainer context) =>
            base.AllowedFor(context) && (context as IEquipmentContainer)?.Holder is IRigged;

        public void OnEquipped(IEquipmentContainer container)
        {
            Ensure.Any.IsNotNull(container, nameof(container));

            var instance = new MeshInstance {Mesh = EquippedMesh};
            var parent = ((IRigged) container.Holder).Skeleton;

            parent?.AddChild(instance);
        }

        public void OnUnequipped(IEquipmentContainer container)
        {
            Ensure.Any.IsNotNull(container, nameof(container));

            var parent = ((IRigged) container.Holder).Skeleton;
            var instance = parent.GetChildren<MeshInstance>().FirstOrDefault(m => m.Mesh == EquippedMesh);

            if (instance != null)
            {
                parent.RemoveChild(instance);
            }
        }
    }
}
