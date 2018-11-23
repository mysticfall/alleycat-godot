using System.Collections.Generic;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Character.Generic;
using AlleyCat.Common;
using AlleyCat.Item;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    public abstract class Character<TRace, TVision, TLocomotion> : GameObject, ICharacter<TRace, TVision, TLocomotion>
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

        public IEquipmentContainer Equipments { get; }

        public Map<string, IAction> Actions { get; }

        public Spatial Spatial { get; }

        public IEnumerable<MeshInstance> Meshes => Skeleton.GetChildComponents<MeshInstance>();

        public AABB Bounds => this.CalculateBounds();

        public Vector3 LabelPosition => _labelMarker.Map(m => m.GlobalTransform.origin).IfNone(this.Center);

        public Map<string, Marker> Markers { get; }

        public Map<string, SkeletonIK> IKChains { get; }

        public bool Visible
        {
            get => Spatial.Visible;
            set => Spatial.Visible = value;
        }

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;

        private Option<Marker> _labelMarker;

        protected Character(
            string key,
            string displayName,
            TRace race,
            Sex sex,
            TVision vision,
            TLocomotion locomotion,
            Skeleton skeleton,
            IAnimationManager animationManager,
            IEnumerable<IAction> actions,
            IEnumerable<Marker> markers,
            Spatial node,
            ILogger logger) : base(logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(race, nameof(race)).IsNotNull();
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
            Actions = actions.ToMap();
            Markers = markers.ToMap();
            Spatial = node;

            IKChains = toMap(Skeleton.GetChildComponents<SkeletonIK>().Map(i => (i.Name, i)));

            var slots = Race.EquipmentSlots.Freeze();
            var equipments = new EquipmentContainer(slots, this, logger);

            equipments.Initialize();
            equipments.DisposeWith(this);

            Equipments = equipments;

            _labelMarker = this.FindLabelMarker();
        }
    }
}
