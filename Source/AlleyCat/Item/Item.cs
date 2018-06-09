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
    public class Item : RigidBody, IItem
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        public Spatial Spatial => this;

        public IEnumerable<MeshInstance> Meshes => this.GetChildren<MeshInstance>();

        public AABB Bounds => Meshes.Select(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2));

        public Vector3 LabelPosition
        {
            get
            {
                if (_labelMarker != null)
                {
                    return _labelMarker.GlobalTransform.origin;
                }

                var bounds = Bounds;

                return GlobalTransform.origin + (bounds.Position + bounds.End) / 2f;
            }
        }

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
