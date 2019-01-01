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

        public Option<Mesh> Mesh { get; set; }

        public Option<Godot.Animation> Animation { get; set; }

        public Option<string> AnimationBlend { get; set; }

        public float AnimationTransition
        {
            get => _animationTransition;
            set => _animationTransition = Mathf.Max(value, 0);
        }

        public Set<string> Tags { get; }

        private float _animationTransition = 1f;

        private readonly BehaviorSubject<bool> _active;

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
        }

        public virtual void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            Optional(holder.AnimationManager)
                .OfType<IAnimationStateManager>()
                .SelectMany(manager => Animation, (manager, animation) => new {manager, animation})
                .SelectMany(t => AnimationBlend.Bind(t.manager.FindBlender),
                    (t, blender) => (t.animation, blender))
                .Iter(t => t.blender.Blend(t.animation, transition: AnimationTransition));

            Mesh
                .SelectMany(mesh => equipment.Meshes, (mesh, instance) => (mesh, instance))
                .Iter(t => t.instance.Mesh = t.mesh);
        }

        public virtual void OnUnequip(IEquipmentHolder holder, Equipment equipment)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            Optional(holder.AnimationManager)
                .OfType<IAnimationStateManager>()
                .Bind(manager => AnimationBlend.Bind(manager.FindBlender))
                .Iter(v => v.Unblend(AnimationTransition));

            foreach (var mesh in equipment.Meshes)
            {
                mesh.Mesh = equipment.ItemMesh;
                mesh.Skeleton = mesh.GetPathTo(equipment.Node);
            }
        }
    }
}
