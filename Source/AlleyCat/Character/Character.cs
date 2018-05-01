using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Character.Generic;
using AlleyCat.Common;
using AlleyCat.IO;
using AlleyCat.Item;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class Character<TVision, TLocomotion> : KinematicBody, ICharacter<TVision, TLocomotion>
        where TVision : class, IVision
        where TLocomotion : class, ILocomotion
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => _displayName;

        public abstract IRace Race { get; }

        public abstract Sex Sex { get; }

        [Service]
        public TVision Vision { get; private set; }

        [Service]
        public TLocomotion Locomotion { get; private set; }

        [Service]
        public IAnimationManager AnimationManager { get; private set; }

        [Service]
        public Skeleton Skeleton { get; private set; }

        [Service]
        public IEquipmentContainer Equipments { get; private set; }

        public IEnumerable<EquipmentSlot> EquipmentSlots => Race?.EquipmentSlots ?? Enumerable.Empty<EquipmentSlot>();

        public Spatial Spatial => this;

        public IEnumerable<MeshInstance> Meshes => Skeleton.GetChildren<MeshInstance>();

        public AABB Bounds => Meshes.Select(m => m.GetAabb()).Aggregate((b1, b2) => b1.Merge(b2));

        [Service]
        protected IRaceRegistry RaceRegistry { get; private set; }

        IVision ISeeing.Vision => Vision;

        ILocomotion ILocomotive.Locomotion => Locomotion;

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        public virtual void SaveState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            var transform = state.GetSection("Transform");

            transform["Translation"] = Translation;
            transform["Rotation"] = Rotation;
        }

        public virtual void RestoreState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            var transform = state.GetSection("Transform");

            if (transform.ContainsKey("Translation"))
            {
                Translation = (Vector3) transform["Translation"];
            }

            if (transform.ContainsKey("Rotation"))
            {
                Rotation = (Vector3) transform["Rotation"];
            }
        }
    }
}
