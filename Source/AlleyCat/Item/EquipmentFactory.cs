using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Morph;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using Enumerable = System.Linq.Enumerable;

namespace AlleyCat.Item
{
    [AutowireContext]
    public class EquipmentFactory : DelegateNodeFactory<Equipment, RigidBody>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Export]
        public string Description { get; set; }

        [Export]
        public EquipmentType EquipmentType { get; set; }

        [Export]
        public Godot.Mesh ItemMesh { get; set; }

        [Service]
        public Option<MeshInstance> Mesh { get; set; }

        [Service(local: true)]
        public IEnumerable<CollisionShape> Colliders { get; set; }

        [Service]
        public IEnumerable<EquipmentConfiguration> Configurations { get; set; }

        [Service(local: true)]
        public IEnumerable<Marker> Markers { get; set; }

        [Node]
        public Option<IMorphGroup> Morphs { get; set; }

        protected override Validation<string, Equipment> CreateService(RigidBody node, ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);
            var description = Description.TrimToOption().Map(Tr);

            return
                from mesh in Mesh
                    .ToValidation("Failed to find the mesh instance.")
                from colliders in Optional(Colliders).Filter(Enumerable.Any)
                    .ToValidation("Failed to find the collision shape.")
                from itemMesh in Optional(ItemMesh)
                    .ToValidation("Failed to find the item mesh.")
                from configurations in Optional(Configurations.Freeze()).Filter(c => Enumerable.Any(c))
                    .ToValidation("Failed to find equipment configuration.")
                select new Equipment(
                    key,
                    displayName,
                    description,
                    EquipmentType,
                    configurations,
                    colliders,
                    mesh,
                    itemMesh,
                    Optional(Markers).Flatten(),
                    Morphs,
                    node,
                    loggerFactory);
        }
    }
}
