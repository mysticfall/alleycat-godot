using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    [Singleton(typeof(EquipmentConfiguration), typeof(ISlotConfiguration), typeof(SlotConfiguration))]
    public abstract class EquipmentConfiguration : SlotConfiguration, ITaggable, IActivatable
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set
            {
                if (_initialized && (this.GetClosestAncestor<Equipment>()?.Equipped ?? false))
                {
                    throw new InvalidOperationException(
                        "Unable to switch configuration while an item is equipped.");
                }

                _active.Value = value;
            }
        }

        public IObservable<bool> OnActiveStateChange => _active;

        [Export]
        public Mesh Mesh { get; set; }

        [Export]
        public Godot.Animation Animation { get; set; }

        public IEnumerable<string> Tags => GetGroups().OfType<string>();

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>();

        private bool _initialized;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _initialized = true;
        }

        public virtual void OnEquip([NotNull] IEquipmentHolder holder, [NotNull] Equipment equipment)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));
            Ensure.Any.IsNotNull(equipment, nameof(equipment));

            if (holder.AnimationManager is IAnimationStateManager animator && Animation != null)
            {
                //TODO Should leave some 'room' for root motion animations so that they can still fire Reset().
                animator.Blend(Animation, 0.99f);
            }

            if (Mesh == null) return;

            foreach (var mesh in equipment.Meshes)
            {
                mesh.Mesh = Mesh;
            }
        }

        public virtual void OnUnequip([NotNull] IEquipmentHolder holder, [NotNull] Equipment equipment)
        {
            Ensure.Any.IsNotNull(holder, nameof(holder));
            Ensure.Any.IsNotNull(equipment, nameof(equipment));

            if (holder.AnimationManager is IAnimationStateManager animator && Animation != null)
            {
                animator.Unblend(Animation.GetName());
            }

            foreach (var mesh in equipment.Meshes)
            {
                mesh.Mesh = equipment.ItemMesh;
                mesh.Skeleton = mesh.GetPathTo(equipment);
            }
        }

        public bool HasTag(string tag) => IsInGroup(tag);

        public void AddTag(string tag) => AddToGroup(tag);

        public void RemoveTag(string tag) => RemoveFromGroup(tag);

        protected override void OnPreDestroy()
        {
            _active?.Dispose();

            base.OnPreDestroy();
        }
    }
}
