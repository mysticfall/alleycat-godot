using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class PickupAction : EquipmentAction
    {
        public float PickupDistance
        {
            get => _pickupDistance;
            set => _pickupDistance = Mathf.Max(value, 0);
        }

        public Option<Godot.Animation> Animation { get; set; }

        public Option<string> IKChain { get; }

        public Set<string> Tags { get; }

        protected Option<string> AnimatorPath { get; }

        protected Option<string> StatesPath { get; }

        protected Option<string> ActionState { get; }

        private float _pickupDistance = 1.2f;

        public PickupAction(
            string key,
            string displayName,
            Set<string> tags,
            Option<string> ikChain,
            Option<string> animatorPath,
            Option<string> statesPath,
            Option<string> actionState,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, active, loggerFactory)
        {
            Tags = tags;
            IKChain = ikChain;
            AnimatorPath = animatorPath;
            StatesPath = statesPath;
            ActionState = actionState;
        }

        protected override void DoExecute(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

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

                    var target = marker.Map(m => m.GlobalTransform).IfNone(equipment.GetGlobalTransform);

                    chain.Iter(c => c.Target = target);
                }

                manager.OnAnimationEvent
                    .Where(e => e.Name == "Action" && e.Argument.Contains(Key))
                    .Take(1)
                    .Subscribe(_ => holder.Equip(equipment, configuration), this);

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

            var action = actor.Actions.Values.Find(a => a is PickupAction);

            action.Match(
                a => a.Execute(new InteractionContext(actor, equipment)),
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor), "The specified actor does not support pick up action."));
        }
    }
}
