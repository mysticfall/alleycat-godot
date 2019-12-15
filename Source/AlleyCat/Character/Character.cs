using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Attribute;
using AlleyCat.Character.Generic;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Item;
using AlleyCat.Logging;
using AlleyCat.Mesh;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    public abstract class Character<TRace, TVision, TLocomotion> : GameNode,
        IDelegateNode<KinematicBody>,
        ICharacter<TRace, TVision, TLocomotion>
        where TRace : Race
        where TVision : class, IVision
        where TLocomotion : class, ILocomotion
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public TRace Race { get; }

        Race ICharacter.Race => Race;

        public Sex Sex { get; }

        public TVision Vision { get; }

        public TLocomotion Locomotion { get; }

        public IAnimationManager AnimationManager { get; }

        public Skeleton Skeleton { get; }

        public IAttributeSet Attributes { get; }

        public IEquipmentContainer Equipments { get; }

        public IActionSet Actions { get; }

        public KinematicBody Node { get; }

        public Spatial Spatial => Node;

        public IEnumerable<MeshInstance> Meshes => Skeleton.GetChildComponents<MeshInstance>();

        public AABB Bounds => this.CalculateBounds();

        public Vector3 LabelPosition => _labelMarker.Map(m => m.GlobalTransform.origin).IfNone(this.Center);

        public Map<string, Marker> Markers { get; }

        public Map<string, SkeletonIK> IKChains { get; }

        public bool Visible
        {
            get => Node.Visible;
            set => Node.Visible = value;
        }

        public IObservable<bool> OnVisibilityChange => Node.OnVisibilityChange();

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;

        private readonly Option<Marker> _labelMarker;

        protected Character(
            string key,
            string displayName,
            TRace race,
            Sex sex,
            IEnumerable<IAttribute> attributes,
            TVision vision,
            TLocomotion locomotion,
            Skeleton skeleton,
            IAnimationManager animationManager,
            IActionSet actions,
            IEnumerable<Marker> markers,
            KinematicBody node,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(race, nameof(race)).IsNotNull();
            Ensure.That(attributes, nameof(attributes)).IsNotNull();
            Ensure.That(vision, nameof(vision)).IsNotNull();
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();
            Ensure.That(animationManager, nameof(animationManager)).IsNotNull();
            Ensure.That(actions, nameof(actions)).IsNotNull();
            Ensure.That(markers, nameof(markers)).IsNotNull();
            Ensure.That(node, nameof(node)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            Race = race;
            Sex = sex;
            Vision = vision;
            Locomotion = locomotion;
            Skeleton = skeleton;
            AnimationManager = animationManager;
            Actions = actions;
            Markers = markers.ToMap();
            Node = node;

            IKChains = toMap(Skeleton.GetChildComponents<SkeletonIK>().Map(i => (i.Name, i)));

            var slots = Race.EquipmentSlots.Freeze();

            Attributes = new AttributeSet(attributes, this, loggerFactory);
            Equipments = new EquipmentContainer(slots, this, loggerFactory);

            _labelMarker = this.FindLabelMarker();

            this.LogInfo("Created a character: Name = '{}', Race = {}, Sex = {}.",
                displayName, race.DisplayName, sex);

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Actions.Values.Iter(a => this.LogDebug("Found action: {}.", a));
                Markers.Values.Iter(m => this.LogDebug("Found marker: {}.", m));
            }
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Seq<object>(Attributes, Equipments).OfType<IInitializable>().Iter(i => i.Initialize());
        }

        protected override void PreDestroy()
        {
            base.PreDestroy();

            Seq<object>(Attributes, Equipments).OfType<IDisposable>().Iter(d => d.Dispose());
        }
    }
}
