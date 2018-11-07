using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Object = Godot.Object;

namespace AlleyCat.Item
{
    public class Equipment : GameObject, ISlotItem<RigidBody>, IMarkable, IEntity
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public virtual Option<string> Description { get; }

        public EquipmentType EquipmentType { get; }

        public string Slot => Configuration.Slot;

        public Set<string> AdditionalSlots => Configuration.AdditionalSlots;

        public EquipmentConfiguration Configuration => ActiveConfiguration.IfNone(_configurations.Head);

        public Option<EquipmentConfiguration> ActiveConfiguration => _configurations.Find(c => c.Active);

        public Map<string, EquipmentConfiguration> Configurations { get; }

        public RigidBody Node { get; }

        public override bool Valid => base.Valid && Object.IsInstanceValid(Node);

        public bool Visible
        {
            get => Node.Visible;
            set => Node.Visible = value;
        }

        public bool Equipped => ActiveConfiguration.IsSome && Node.FindClosestAncestor<IEquipmentHolder>().IsSome;

        public Spatial Spatial => Node;

        public Mesh ItemMesh { get; }

        public MeshInstance Mesh { get; }

        public CollisionShape Shape { get; }

        public IEnumerable<MeshInstance> Meshes => Seq1(Mesh).Filter(m => m.Visible);

        public AABB Bounds => Meshes.Select(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2));

        public Map<string, Marker> Markers { get; }

        public Vector3 LabelPosition => _labelMarker.Map(m => m.GlobalTransform.origin).IfNone(this.Center);

        Node ISlotItem.Node => Node;

        private readonly IEnumerable<EquipmentConfiguration> _configurations;

        private Option<Marker> _labelMarker;

        public Equipment(
            string key,
            string displayName,
            Option<string> description,
            EquipmentType equipmentType,
            IEnumerable<EquipmentConfiguration> configurations,
            RigidBody node,
            CollisionShape shape,
            MeshInstance mesh,
            Mesh itemMesh,
            IEnumerable<Marker> markers)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(configurations, nameof(configurations)).IsNotNull();
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(shape, nameof(shape)).IsNotNull();
            Ensure.That(mesh, nameof(mesh)).IsNotNull();
            Ensure.That(itemMesh, nameof(itemMesh)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            Description = description;
            EquipmentType = equipmentType;
            Node = node;
            Mesh = mesh;
            Shape = shape;
            ItemMesh = itemMesh;

            Markers = markers.ToMap();

            _configurations = configurations.Freeze();

            Ensure.Enumerable.HasItems(_configurations, nameof(configurations));

            Configurations = _configurations.ToMap();

            _configurations.ToObservable()
                .SelectMany(c => c.OnActiveStateChange.Where(identity).Select(_ => c))
                .SelectMany(active => _configurations.Where(c => c != active && c.Active))
                .Subscribe(c => c.Deactivate())
                .AddTo(this);

            _labelMarker = this.FindLabelMarker();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            UpdateEquipState(Equipped);
        }

        public virtual void Equip(IEquipmentHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            UpdateEquipState(true);

            Configuration.OnEquip(holder, this);
        }

        public virtual void Unequip(IEquipmentHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            Configuration.OnUnequip(holder, this);

            UpdateEquipState(false);
        }

        private void UpdateEquipState(bool equipped)
        {
            Node.Mode = equipped ? RigidBody.ModeEnum.Kinematic : RigidBody.ModeEnum.Rigid;
            Node.Sleeping = equipped;
            Node.InputRayPickable = !equipped;

            Shape.Disabled = equipped;
        }

        public bool AllowedFor(ISlotContainer context) => true;

        public bool AllowedFor(object context) => true;
    }
}
