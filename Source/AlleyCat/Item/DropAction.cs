using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class DropAction : EquipmentAction
    {
        public Option<Godot.Animation> Animation { get; set; }

        protected Option<string> AnimatorPath { get; }

        protected Option<string> StatesPath { get; }

        protected Option<string> ActionState { get; }

        public DropAction(
            string key,
            string displayName,
            Option<string> animatorPath,
            Option<string> statesPath,
            Option<string> actionState,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, active, loggerFactory)
        {
            AnimatorPath = animatorPath;
            StatesPath = statesPath;
            ActionState = actionState;
        }

        protected override void DoExecute(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            var configuration = equipment.Configuration;
            var dropTo = holder.Spatial.GetCurrentScene().Map(s => s.ItemsRoot);

            var args =
                from manager in Optional(holder).OfType<IAnimatable>().Map(a => a.AnimationManager)
                from animation in configuration.UnequipAnimation.Concat(Animation).HeadOrNone()
                select (manager, animation);

            args.HeadOrNone().Match(
                v => PlayAnimation(holder, equipment, dropTo, v.animation, v.manager, context),
                () => Unequip(holder, equipment, dropTo, context)
            );
        }


        protected virtual void PlayAnimation(
            IEquipmentHolder holder,
            Equipment equipment,
            Option<Node> dropTo,
            Godot.Animation animation,
            IAnimationManager animationManager,
            InteractionContext context)
        {
            animationManager.OnAnimationEvent
                .OfType<TriggerEvent>()
                .Where(e => e.Name == "Action" && e.Argument.Contains(Key))
                .Take(1)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(_ => Unequip(holder, equipment, dropTo, context), this);

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

        protected virtual void Unequip(
            IEquipmentHolder holder,
            Equipment equipment,
            Option<Node> dropTo,
            InteractionContext context)
        {
            holder.Unequip(equipment, dropTo);
        }

        protected override bool AllowedFor(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();
            Ensure.That(equipment, nameof(equipment)).IsNotNull();

            return holder.HasEquipment(equipment.Slot) && equipment.Configuration.HasTag(Carry);
        }
    }

    public static class DropActionExtensions
    {
        public static void Drop<T>(this T actor, Equipment equipment)
            where T : class, IActor, IEquipmentHolder
        {
            Ensure.That(actor, nameof(actor)).IsNotNull();

            var action = actor.Actions.Values.Find(a => a is DropAction);

            action.Match(
                a => a.Execute(new InteractionContext(actor, equipment)),
                () => throw new ArgumentOutOfRangeException(
                    nameof(actor), "The specified actor does not support drop action.")
            );
        }
    }
}
