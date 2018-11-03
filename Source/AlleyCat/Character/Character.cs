using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Character.Generic;
using AlleyCat.Common;
using AlleyCat.Item;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class Character<TVision, TLocomotion> : KinematicBody, ICharacter<TVision, TLocomotion>
        where TVision : class, IVision
        where TLocomotion : class, ILocomotion
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        public virtual string DisplayName => _displayName.TrimToOption().Map(Tr).IfNone(() => Key);

        public abstract IRace Race { get; }

        public abstract Sex Sex { get; }

        public TVision Vision => (TVision) _vision;

        public TLocomotion Locomotion => (TLocomotion) _locomotion;

        public IAnimationManager AnimationManager => _animationManager.Head();

        public Skeleton Skeleton => (Skeleton) _skeleton;

        public IEquipmentContainer Equipments => _equipments.Head();

        public Map<string, IAction> Actions { get; private set; } = Map<string, IAction>();

        public Spatial Spatial => this;

        public IEnumerable<MeshInstance> Meshes => _skeleton.SelectMany(s => s.GetChildComponents<MeshInstance>());

        public AABB Bounds => this.CalculateBounds();

        public Vector3 LabelPosition =>
            _labelMarker.Map(m => m.GlobalTransform.origin).IfNone(this.Center);

        public Map<string, Marker> Markers { get; private set; } = Map<string, Marker>();

        public Map<string, SkeletonIK> IKChains { get; private set; } = Map<string, SkeletonIK>();

        public virtual bool Valid => IsInstanceValid(this) &&
                                     _vision.IsSome &&
                                     _locomotion.IsSome &&
                                     _animationManager.IsSome &&
                                     _skeleton.IsSome &&
                                     _equipments.IsSome &&
                                     _raceRegistry.IsSome;

        protected IRaceRegistry RaceRegistry => _raceRegistry.Head();

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;

        [Export] private string _key;

        [Export] private string _displayName;

        [Service] private Option<TVision> _vision;

        [Service] private Option<TLocomotion> _locomotion;

        [Service] private Option<IAnimationManager> _animationManager;

        [Service] private Option<Skeleton> _skeleton;

        [Service] private Option<IEquipmentContainer> _equipments;

        [Service] private Option<IRaceRegistry> _raceRegistry;

        [Service] private Option<IReadOnlyDictionary<string, IAction>> _actions;

        [Service(false, false)] private IEnumerable<Marker> _markers = Enumerable.Empty<Marker>();

        private Option<Marker> _labelMarker;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            IKChains = toMap(Skeleton.GetChildComponents<SkeletonIK>().Map(i => (i.Name, i)));

            Actions = _actions.SelectMany(a => a.Values).ToMap();
            Markers = _markers.ToMap();

            _labelMarker = this.FindLabelMarker();
        }
    }
}
