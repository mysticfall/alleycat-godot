using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager), typeof(IAnimationStateManager))]
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        public const string OverrideNodePrefix = "Override ";

        public const string OverrideBlendNodePrefix = "Override Blend ";

        private IDictionary<int, string> _overrides = new Dictionary<int, string>(0);

        private int _overridableSlots;

        [Service]
        public AnimationTreePlayer TreePlayer { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TreePlayer.Active = false;

            _overridableSlots = TreePlayer.GetNodeList().Count(n => n.StartsWith(OverrideBlendNodePrefix));
            _overrides = Enumerable
                .Range(1, _overridableSlots)
                .Select(i => (i, OverrideNodePrefix + i))
                .Select(t => (t.Item1, TreePlayer.AnimationNodeGetAnimation(t.Item2)?.GetName()))
                .Where(t => t.Item2 != null)
                .ToDictionary(t => t.Item1, t => t.Item2);
        }

        public override void Advance(float delta)
        {
            if (!PlayingOneShotAnimation)
            {
                TreePlayer.Advance(0);
            }

            base.Advance(delta);
        }

        protected override void ProcessFrames(float delta) => TreePlayer.Advance(delta);

        public void Blend(Godot.Animation animation, float influence = 1f)
        {
            Ensure.Any.IsNotNull(animation, nameof(animation));

            var slot = Enumerable
                .Range(1, _overridableSlots)
                .FirstOrDefault(i => !_overrides.ContainsKey(i));

            if (slot == default(int))
            {
                throw new InvalidOperationException(
                    $"No overridable slots left: total = {_overridableSlots}.");
            }

            _overrides[slot] = animation.GetName();

            TreePlayer.AnimationNodeSetAnimation(OverrideNodePrefix + slot, animation);
            TreePlayer.Blend2NodeSetAmount(OverrideBlendNodePrefix + slot, influence);
        }

        public void Unblend(string animation)
        {
            Ensure.Any.IsNotNull(animation, nameof(animation));

            var slot = _overrides.FirstOrDefault(i => i.Value == animation).Key;

            if (slot == default(int)) return;

            TreePlayer.Blend2NodeSetAmount(OverrideBlendNodePrefix + slot, 0);
            TreePlayer.AnimationNodeSetAnimation(OverrideNodePrefix + slot, null);

            _overrides.Remove(slot);
        }
    }
}
