using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class PickupAction : EquipmentAction
    {
        [Export(PropertyHint.ExpRange, "0.1, 5")]
        public float PickupDistance
        {
            get => _pickupDistance;
            set
            {
                Ensure.That(value, nameof(value)).IsGt(0);

                _pickupDistance = value;
            }
        }

        public Option<Godot.Animation> Animation
        {
            get => Optional(_animation);
            set => _animation = value.ValueUnsafe();
        }

        public Option<string> IKChain
        {
            get => _ikChain.TrimToOption();
            set => _ikChain = value.ValueUnsafe();
        }

        public Set<string> Tags => toSet(_tags);

        protected Option<string> AnimatorPath => _animatorPath.TrimToOption();

        protected Option<string> StatesPath => _statesPath.TrimToOption();

        protected Option<string> ActionState => _actionState.TrimToOption();

        [Export, UsedImplicitly] private Godot.Animation _animation;

        [Export, UsedImplicitly] private string _animatorPath = "States/Action";

        [Export, UsedImplicitly] private string _statesPath = "States";

        [Export, UsedImplicitly] private string _actionState = "Action";

        [Export, UsedImplicitly] private string _ikChain = "Right Hand IK";

        [Export, UsedImplicitly] private Array<string> _tags = new Array<string> {Carry, Hand};

        private float _pickupDistance = 1.2f;

        protected override void DoExecute(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            holder.FindEquipConfiguration(equipment, Tags.ToArray()).Iter(ExecuteWithConfiguration);

            void ExecuteWithConfiguration(EquipmentConfiguration configuration)
            {
                var animationArguments = Optional(holder)
                    .OfType<IAnimatable>()
                    .Map(a => a.AnimationManager)
                    .SelectMany(manager => Animation, 
                        (manager, animation) => (animation, manager))
                    .HeadOrNone();

                animationArguments.Match(
                    t => ExecuteWithAnimation(configuration, t.animation, t.manager),
                    () => holder.Equip(equipment, configuration)
                );
            }

            void ExecuteWithAnimation(
                EquipmentConfiguration configuration,
                Godot.Animation animation,
                IAnimationManager manager)
            {
                if (holder is IRigged rig)
                {
                    var chain = IKChain.Bind(c => rig.IKChains.Find(c));
                    var marker = equipment.Markers.Find(configuration.Key);

                    var target = marker.Map(m => m.GlobalTransform).IfNone(equipment.GlobalTransform);

                    chain.Iter(c => c.Target = target);
                }

                manager.OnAnimationEvent
                    .Where(e => e.Name == "Action" && e.Argument.Contains(Key))
                    .Take(1)
                    .Subscribe(_ => holder.Equip(equipment, configuration))
                    .AddTo(this);

                if (manager is IAnimationStateManager stateManager &&
                    AnimatorPath.IsSome && StatesPath.IsSome)
                {
                    (
                        from animator in AnimatorPath.Bind(stateManager.FindAnimator)
                        from states in StatesPath.Bind(stateManager.FindStates)
                        select (animator, states)).Iter(t =>
                    {
                        t.animator.Animation = animation;
                        ActionState.Iter(t.states.Playback.Travel);
                    });
                }
                else
                {
                    manager.Play(animation);
                }
            }
        }

        protected override bool AllowedFor(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            return !equipment.Equipped && holder.DistanceTo(equipment) <= PickupDistance;
        }
    }

    public static class PickupActionExtensions
    {
        public static void Pickup<T>(this T actor, Equipment equipment)
            where T : class, IActor, IEquipmentHolder
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            var action = actor.Actions.Values.Find(a => a is PickupAction);

            action.Match(
                a => a.Execute(new InteractionContext(actor, equipment)),
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor), "The specified actor does not support pick up action."));
        }
    }
}
