using System;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public class AttachedEquipment : Equipment
    {
        [Node]
        public MeshInstance Mesh { get; private set; }

        [Export, UsedImplicitly] private NodePath _mesh;

        public override void Equip(IEquipmentHolder holder)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));

            if (holder.Markers.TryGetValue(Slot, out var marker))
            {
                Mesh.GetParent()?.RemoveChild(Mesh);

                marker.AddChild(Mesh);

                _mesh = Mesh.GetPath();
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(holder),
                    $"The equipment holder does not have a proper attach point: '{Slot}'.");
            }
        }

        public override void Unequip()
        {
            Mesh.GetParent()?.RemoveChild(Mesh);

            AddChild(Mesh);

            _mesh = Mesh.GetPath();
        }
    }
}
