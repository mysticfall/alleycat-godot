using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

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
                if (_initialized && this.FindClosestAncestor<Equipment>().Exists(e => e.Equipped))
                {
                    throw new InvalidOperationException(
                        "Unable to switch configuration while an item is equipped.");
                }

                _active.OnNext(value);
            }
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public Option<Mesh> Mesh
        {
            get => Optional(_mesh);
            set => _mesh = value.ValueUnsafe();
        }

        public Option<Godot.Animation> Animation
        {
            get => Optional(_animation);
            set => _animation = value.ValueUnsafe();
        }

        public Option<string> AnimationBlend
        {
            get => _animationBlend.TrimToOption();
            set => _animationBlend = value.ValueUnsafe();
        }

        [Export(PropertyHint.ExpRange, "0,10")]
        public float AnimationTransition
        {
            get => _animationTransition;
            set => _animationTransition = Mathf.Min(value, 0);
        }

        public Set<string> Tags { get; private set; } = Set<string>();

        [Export] private Mesh _mesh;

        [Export] private Godot.Animation _animation;

        [Export] private string _animationBlend;

        private float _animationTransition = 1f;

        private readonly BehaviorSubject<bool> _active;

        private bool _initialized;

        protected EquipmentConfiguration()
        {
            _active = new BehaviorSubject<bool>(false).AddTo(this);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _initialized = true;

            Tags = toSet(GetGroups().OfType<string>());
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
                mesh.Skeleton = mesh.GetPathTo(equipment);
            }
        }

        public bool HasTag(string tag)
        {
            Ensure.That(tag, nameof(tag)).IsNotNull();

            return IsInGroup(tag);
        }
    }
}
