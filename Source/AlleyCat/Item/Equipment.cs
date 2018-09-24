using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.IO;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    [AutowireContext]
    public class Equipment : RigidBody, ISlotItem, IMarkable, IEntity
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        public virtual string Description => Tr(_description.TrimToEmpty());

        [Export, UsedImplicitly]
        public EquipmentType EquipmentType { get; private set; }

        public string Slot => Configuration?.Slot;

        public IEnumerable<string> AdditionalSlots => Configuration?.AdditionalSlots ?? Enumerable.Empty<string>();

        public EquipmentConfiguration Configuration => Configurations.Values.FirstOrDefault(c => c.Active);

        public IReadOnlyDictionary<string, EquipmentConfiguration> Configurations { get; private set; } =
            Enumerable.Empty<EquipmentConfiguration>().ToDictionary();

        public virtual bool Valid => IsInstanceValid(this);

        public bool Equipped => Configuration != null && GetParent() != null;

        public Spatial Spatial => _mesh;

        public Mesh ItemMesh => _itemMesh;

        [Service]
        public CollisionShape Shape { get; private set; }

        public IEnumerable<MeshInstance> Meshes =>
            _mesh != null && _mesh.Visible ? new[] {_mesh} : Enumerable.Empty<MeshInstance>();

        public AABB Bounds => Meshes.Select(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2));

        public IReadOnlyDictionary<string, Marker> Markers { get; private set; } =
            Enumerable.Empty<Marker>().ToDictionary();

        public Vector3 LabelPosition => _labelMarker?.GlobalTransform.origin ?? this.Center();

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;
        
        [Export, UsedImplicitly] private string _description;

        [Export, UsedImplicitly] private Mesh _itemMesh;

        [Service] private MeshInstance _mesh;

        [Service] private IEnumerable<EquipmentConfiguration> _configurations;

        [Service(false)] private IEnumerable<Marker> _markers;

        private Marker _labelMarker;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Configurations = _configurations.ToDictionary();

            var configurations = Configurations.Values.ToList();

            configurations.ToObservable()
                .SelectMany(c => c.OnActiveStateChange.Where(s => s).Select(_ => c))
                .SelectMany(active => configurations.Where(c => c != active && c.Active))
                .Subscribe(c => c.Deactivate())
                .AddTo(this);

            if (_markers != null)
            {
                Markers = _markers.ToDictionary(m => m.Key);
            }

            _labelMarker = this.GetLabelMarker();

            UpdateEquipState(Equipped);
        }

        public virtual void Equip(IEquipmentHolder holder)
        {
            UpdateEquipState(true);

            Configuration.OnEquip(holder, this);
        }

        public virtual void Unequip(IEquipmentHolder holder)
        {
            Configuration.OnUnequip(holder, this);

            UpdateEquipState(false);
        }

        private void UpdateEquipState(bool equipped)
        {
            Mode = equipped ? ModeEnum.Kinematic : ModeEnum.Rigid;
            Sleeping = equipped;
            Shape.Disabled = equipped;
            InputRayPickable = !equipped;
        }

        public bool AllowedFor(ISlotContainer context) => true;

        public bool AllowedFor(object context) => true;

        public virtual void SaveState(IState state)
        {
        }

        public virtual void RestoreState(IState state)
        {
        }
    }
}
