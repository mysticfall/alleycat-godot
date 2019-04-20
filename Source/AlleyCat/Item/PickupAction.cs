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

            holder.FindEquipConfiguration(equipment, Tags.ToArray()).Iter(WithConfiguration);

            void WithConfiguration(EquipmentConfiguration configuration)
            {
                var args =
                    from manager in Optional(holder).OfType<IAnimatable>().Map(a => a.AnimationManager)
                    from animation in configuration.EquipAnimation.Concat(Animation).HeadOrNone()
                    select (manager, animation);

                args.HeadOrNone().Match(
                    v => PlayAnimation(holder, equipment, configuration, v.animation, v.manager, context),
                    () => Equip(holder, equipment, configuration, context)
                );
            }
        }

        protected virtual void PlayAnimation(
            IEquipmentHolder holder,
            Equipment equipment,
            EquipmentConfiguration configuration,
            Godot.Animation animation,
            IAnimationManager animationManager,
            InteractionContext context)
        {
            if (holder is IRigged rig)
            {
                var chain = IKChain.Bind(c => rig.IKChains.Find(c));
                var marker = equipment.Markers.Find(configuration.Key);

                var target = marker.Map(m => m.GlobalTransform).IfNone(equipment.GetGlobalTransform);

                chain.Iter(c => c.Target = target);
            }

            animationManager.OnAnimationEvent
                .OfType<TriggerEvent>()
                .Where(e => e.Name == "Action" && e.Argument.Contains(Key))
                .Take(1)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(_ => Equip(holder, equipment, configuration, context), this);

            if (animationManager is IAnimationStateManager stateManager &&
                AnimatorPath.IsSome && StatesPath.IsSome)
            {
                (
                    from animator in AnimatorPath.Bind(stateManager.FindAnimator)
                    from states in StatesPath.Bind(stateManager.FindStates)
                    from state in ActionState 
                    select (animator, states, state)).Iter(t =>
                {
                    t.animator.Animation = animation;
                    t.states.Playback.Travel(t.state);
                });
            }
            else
            {
                animationManager.Play(animation);
            }
        }

        protected virtual void Equip(
            IEquipmentHolder holder,
            Equipment equipment,
            EquipmentConfiguration configuration,
            InteractionContext context)
        {
            holder.Equip(equipment, configuration);
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
