using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    [AutowireContext]
    public class Equipment : RigidBody, ISlotItem, IMarkable, IEntity
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        public virtual string DisplayName => _displayName.TrimToOption().Map(Tr).IfNone(() => Key);

        public virtual Option<string> Description => _description.TrimToOption().Map(Tr);

        [Export, UsedImplicitly]
        public EquipmentType EquipmentType { get; private set; }

        public string Slot => Configuration.Slot;

        public Set<string> AdditionalSlots => Configuration.AdditionalSlots;

        public EquipmentConfiguration Configuration => ActiveConfiguration.IfNone(_configurations.Head);

        public Option<EquipmentConfiguration> ActiveConfiguration => _configurations.Find(c => c.Active);

        public Map<string, EquipmentConfiguration> Configurations { get; private set; } =
            Map<string, EquipmentConfiguration>();

        public virtual bool Valid => IsInstanceValid(this);

        public bool Equipped => ActiveConfiguration.IsSome && 
                                this.GetAncestors().Exists(a => a is IEquipmentHolder);

        public Spatial Spatial => (Spatial) _mesh;

        public Mesh ItemMesh => Some(_itemMesh).Head();

        public CollisionShape Shape => (CollisionShape) _shape;

        public IEnumerable<MeshInstance> Meshes => _mesh.Filter(m => m.Visible);

        public AABB Bounds => Meshes.Select(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2));

        public Map<string, Marker> Markers { get; private set; } = Map<string, Marker>();

        public Vector3 LabelPosition => _labelMarker.Map(m => m.GlobalTransform.origin).IfNone(this.Center);

        [Export] private string _key;

        [Export] private string _displayName;

        [Export] private string _description;

        [Export] private Mesh _itemMesh;

        [Service] private Option<MeshInstance> _mesh;

        [Service] private Option<CollisionShape> _shape;

        [Service] private IEnumerable<EquipmentConfiguration> _configurations = Seq<EquipmentConfiguration>();

        [Service(false, false)] private IEnumerable<Marker> _markers = Seq<Marker>();

        private Option<Marker> _labelMarker;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Configurations = _configurations.ToMap();

            var configurations = Configurations.Values.ToList();

            configurations.ToObservable()
                .SelectMany(c => c.OnActiveStateChange.Where(s => s).Select(_ => c))
                .SelectMany(active => configurations.Where(c => c != active && c.Active))
                .Subscribe(c => c.Deactivate())
                .AddTo(this.GetCollector());

            if (_markers != null)
            {
                Markers = toMap(_markers.Map(m => (m.Key, m)));
            }

            _labelMarker = this.FindLabelMarker();

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
            Mode = equipped ? ModeEnum.Kinematic : ModeEnum.Rigid;
            Sleeping = equipped;
            Shape.Disabled = equipped;
            InputRayPickable = !equipped;
        }

        public bool AllowedFor(ISlotContainer context) => true;

        public bool AllowedFor(object context) => true;
    }
}
