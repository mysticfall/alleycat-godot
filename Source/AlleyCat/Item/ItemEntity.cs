using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.IO;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    [AutowireContext]
    public class ItemEntity : RigidBody, IItemEntity, IMarkable
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        public bool Valid => !IsQueuedForDeletion();

        public Spatial Spatial => this;

        public IEnumerable<MeshInstance> Meshes => this.GetChildren<MeshInstance>();

        public AABB Bounds => this.CalculateBounds();

        public Vector3 LabelPosition => _labelMarker?.GlobalTransform.origin ?? this.Center();

        public IEnumerable<IAction> Actions => _actions ?? Enumerable.Empty<IAction>();

        public IReadOnlyDictionary<string, Marker> Markers { get; private set; } =
            Enumerable.Empty<Marker>().ToDictionary();

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        [Service(false)] private IEnumerable<IAction> _actions;

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
            if (_markers != null)
            {
                Markers = _markers.ToDictionary(m => m.Key);
            }

            _labelMarker = this.GetLabelMarker();
        }

        public void SaveState(IState state)
        {
        }

        public void RestoreState(IState state)
        {
        }
    }
}
