using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using AlleyCat.Autowire;
using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager), typeof(IAnimationStateManager))]
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        public const string OneShotNode = "One Shot";

        public const string OneShotTriggerNode = "One Shot Trigger";

        public const string OverrideNodePrefix = "Override ";

        public const string OverrideBlendNodePrefix = "Override Blend ";

        private IScheduler _scheduler;

        private IDictionary<int, string> _overrides = new Dictionary<int, string>(0);

        private int _overridableSlots;

        private IDisposable _oneShotAnimationCallback;

        [Service]
        public AnimationTreePlayer TreePlayer { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TreePlayer.Active = false;

            _scheduler = this.GetScheduler(ProcessMode);

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
            TreePlayer.Advance(0);

            base.Advance(delta);
        }

        protected override void ProcessFrames(float delta) => TreePlayer.Advance(delta);

        public override void Play(Godot.Animation animation, System.Action onFinish = null)
        {
            _oneShotAnimationCallback?.Dispose();

            TreePlayer.AnimationNodeSetAnimation(OneShotNode, animation);
            TreePlayer.OneshotNodeStart(OneShotTriggerNode);

            if (onFinish != null)
            {
                _oneShotAnimationCallback = _scheduler?.Schedule(
                    TimeSpan.FromSeconds(animation.Length), onFinish);
            }
        }

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

        protected override void Dispose(bool disposing)
        {
            _oneShotAnimationCallback?.Dispose();
            _oneShotAnimationCallback = null;

            base.Dispose(disposing);
        }
    }
}
