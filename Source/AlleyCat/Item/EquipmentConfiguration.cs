using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class EquipmentConfiguration : SlotConfiguration, ITaggable, IActivatable
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public Option<Godot.Mesh> Mesh { get; set; }

        public Option<Godot.Animation> EquipAnimation { get; set; }

        public Option<Godot.Animation> Animation { get; set; }

        public Option<Godot.Animation> UnequipAnimation { get; set; }

        public Option<string> AnimationBlend { get; set; }

        public float AnimationTransition
        {
            get => _animationTransition;
            set => _animationTransition = Mathf.Max(value, 0);
        }

        public Set<string> Tags { get; }

        public Option<IEquipmentHolder> Holder => _holder.Value;

        public IObservable<Option<IEquipmentHolder>> OnHolderChange => _holder.AsObservable();

        private float _animationTransition = 1f;

        private readonly BehaviorSubject<bool> _active;

        private readonly BehaviorSubject<Option<IEquipmentHolder>> _holder;

        protected EquipmentConfiguration(
            string key,
            string slot,
            Set<string> additionalSlots,
            Set<string> tags,
            bool active,
            ILoggerFactory loggerFactory) : base(key, slot, additionalSlots, loggerFactory)
        {
            Tags = tags;

            _active = CreateSubject(active);
            _holder= CreateSubject(Option<IEquipmentHolder>.None);
        }

        public virtual void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            Logger.LogDebug($"Equipping {equipment} on {holder}.");

            Optional(holder.AnimationManager)
                .OfType<IAnimationStateManager>()
                .SelectMany(manager => Animation, (manager, animation) => new {manager, animation})
                .SelectMany(t => AnimationBlend.Bind(t.manager.FindBlender),
                    (t, blender) => (t.animation, blender))
                .Iter(t => t.blender.Blend(t.animation, transition: AnimationTransition));

            Mesh
                .SelectMany(mesh => equipment.Meshes, (mesh, instance) => (mesh, instance))
                .Iter(t => t.instance.Mesh = t.mesh);

            _holder.OnNext(Some(holder));
        }

        public virtual void OnUnequip(IEquipmentHolder holder, Equipment equipment)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            Logger.LogDebug($"Unequipping {equipment} from {holder}.");

            Optional(holder.AnimationManager)
                .OfType<IAnimationStateManager>()
                .Bind(manager => AnimationBlend.Bind(manager.FindBlender))
                .Iter(v => v.Unblend(AnimationTransition));

            foreach (var mesh in equipment.Meshes)
            {
                mesh.Mesh = equipment.ItemMesh;
                mesh.Skeleton = mesh.GetPathTo(equipment.Node);
            }

            _holder.OnNext(None);
        }
    }
}
