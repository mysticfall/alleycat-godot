using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class PickupAction : EquipmentAction
    {
        [Export(PropertyHint.ExpRange, "0.1, 5")]
        public float PickupDistance { get; set; } = 1.2f;

        [Export]
        public Godot.Animation Animation { get; set; }

        [Export]
        public string IKChain { get; set; } = "Right Hand IK";

        public IEnumerable<string> Tags => _tags.TrimToEnumerable();

        [Export]
        protected string AnimatorPath { get; private set; } = "States/Action";

        [Export]
        protected string StatesPath { get; private set; } = "States";

        [Export]
        protected string ActionState { get; private set; } = "Action";

        [Export, UsedImplicitly] private string _tags = string.Join(",", Carry, Hand);

        protected override void DoExecute(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context)
        {
            var configuration = holder.FindEquipConfiguration(equipment, Tags.ToArray());

            if (configuration == null) return;

            if (Animation == null || !(holder is IAnimatable animatable))
            {
                holder.Equip(equipment, configuration);

                return;
            }

            if (IKChain != null &&
                holder is IRigged rig &&
                rig.IKChains.TryGetValue(IKChain, out var chain))
            {
                equipment.Markers.TryGetValue(configuration.Key, out var marker);

                chain.Target = marker?.GlobalTransform ?? equipment.GlobalTransform;
            }

            var animationManager = animatable.AnimationManager;

            animationManager.OnAnimationEvent
                .Where(e => e.Name == "Action" && (string) e.Argument == Key)
                .Take(1)
                .Subscribe(_ => holder.Equip(equipment, configuration))
                .AddTo(this);

            if (!(animationManager is IAnimationStateManager stateManager) ||
                string.IsNullOrEmpty(AnimatorPath) ||
                string.IsNullOrEmpty(StatesPath))
            {
                animationManager.Play(Animation);
            }
            else
            {
                var animator = stateManager.GetAnimator(AnimatorPath);
                var states = stateManager.GetStates(StatesPath);

                if (animator == null || states == null) return;

                animator.Animation = Animation;
                states.Playback.Travel(ActionState);
            }
        }

        protected override bool AllowedFor(
            IEquipmentHolder holder, Equipment equipment, InteractionContext context) =>
            !equipment.Equipped && holder.DistanceTo(equipment) <= PickupDistance;
    }

    public static class PickupActionExtensions
    {
        public static void Pickup<T>([NotNull] this T actor, [NotNull] Equipment equipment)
            where T : IActor, IEquipmentHolder
        {
            Ensure.Any.IsNotNull(actor, nameof(actor));
            Ensure.Any.IsNotNull(equipment, nameof(equipment));

            var action = actor.Actions.Values.FirstOrDefault(a => a is PickupAction);
            var context = new InteractionContext(actor, equipment);

            if (action != null && action.AllowedFor(context))
            {
                action.Execute(context);
            }
        }
    }
}
