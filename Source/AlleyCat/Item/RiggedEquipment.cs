using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public class RiggedEquipment : Equipment
    {
        [Node]
        public MeshInstance Mesh { get; private set; }

        [Export, UsedImplicitly] private NodePath _mesh;

        public override void Equip(IEquipmentHolder holder)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            Mesh.GetParent()?.RemoveChild(Mesh);

            holder.Skeleton.AddChild(Mesh);

            _mesh = Mesh.GetPath();
        }

        public override void Unequip()
        {
            Mesh.GetParent()?.RemoveChild(Mesh);

            AddChild(Mesh);

            _mesh = Mesh.GetPath();
        }
    }
}
