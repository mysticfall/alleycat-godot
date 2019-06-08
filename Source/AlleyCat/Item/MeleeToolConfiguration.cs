using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class MeleeToolConfiguration : AttachedConfiguration
    {
        public Godot.Animation SwingAnimation { get; }

        protected IPlayerControl PlayerControl { get; }

        protected virtual IObservable<bool> ArmInput { get; }

        protected virtual IObservable<Vector2> SwingInput { get; }

        protected IEnumerable<IInput> ConflictingInputs { get; }

        protected string StatesPath { get; }

        protected string SeekerPath { get; }

        protected string IdleState { get; }

        protected string SwingState { get; }

        protected Range<float> AnimationRange { get; }

        protected IObservable<IEquipmentHolder> OnPlayerChange => OnHolderChange
            .CombineLatest(PlayerControl.OnCharacterChange, (h, p) => h.Filter(v => p.Contains(v)))
            .Select(p => p.ToObservable())
            .Switch();

        protected IObservable<Unit> OnArm => OnPlayerChange
            .Select(_ => ArmInput.Where(identity))
            .Switch()
            .AsUnitObservable();

        protected IObservable<Unit> OnDisarm => Merge(
            ArmInput.Where(v => !v).AsUnitObservable(),
            OnHolderChange.Where(v => v.IsNone).AsUnitObservable(),
            Disposed.Where(identity).AsUnitObservable());

        public MeleeToolConfiguration(
            string key,
            string slot,
            Set<string> additionalSlots,
            Set<string> tags,
            Option<IInputBindings> armInput,
            Option<IInputBindings> swingInput,
            Godot.Animation swingAnimation,
            string statesPath,
            string seekerPath,
            string idleState,
            string swingState,
            Range<float> animationRange,
            IPlayerControl playerControl,
            bool active,
            ILoggerFactory loggerFactory) : base(key, slot, additionalSlots, tags, active, loggerFactory)
        {
            Ensure.That(swingAnimation, nameof(swingAnimation)).IsNotNull();
            Ensure.That(statesPath, nameof(statesPath)).IsNotNullOrEmpty();
            Ensure.That(seekerPath, nameof(seekerPath)).IsNotNullOrEmpty();
            Ensure.That(idleState, nameof(idleState)).IsNotNullOrEmpty();
            Ensure.That(swingState, nameof(swingState)).IsNotNullOrEmpty();
            Ensure.That(playerControl, nameof(playerControl)).IsNotNull();

            SwingInput = swingInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Empty<Vector2>)
                .Where(_ => Valid)
                .Select(v => v * 2f);
            ArmInput = armInput.Bind(i => i.FindTrigger().HeadOrNone())
                .MatchObservable(identity, Empty<bool>)
                .Where(_ => Valid);

            SwingAnimation = swingAnimation;
            StatesPath = statesPath;
            SeekerPath = seekerPath;
            IdleState = idleState;
            SwingState = swingState;
            AnimationRange = animationRange;
            PlayerControl = playerControl;

            bool Conflicts(IInput input) => swingInput.Bind(i => i.Inputs.Values).Exists(input.ConflictsWith);

            ConflictingInputs = playerControl.Inputs.Filter(i => i.Active).Filter(Conflicts);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var minPos = AnimationRange.Min;
            var maxPos = AnimationRange.Max > AnimationRange.Min
                ? AnimationRange.Max
                : Animation.Map(a => a.Length).IfNone(1f);

            Logger.LogDebug($"Using animation range: {minPos} - {maxPos}.");

            var manager = OnPlayerChange
                .Select(p => p.AnimationManager)
                .OfType<IAnimationStateManager>();

            var value = SwingInput
                .Scan(0f, (s, v) => Mathf.Clamp(s + v.y, 0f, 600f))
                .Select(v => v / 600f * (maxPos - minPos) + minPos);

            var states = manager
                .Select(m => m.FindStates(StatesPath).ToObservable())
                .Switch();

            var seeker = manager
                .Select(m => m.FindSeekableAnimator(SeekerPath).ToObservable())
                .Switch();

            var blender = manager
                .Select(m => AnimationBlend.Bind(m.FindBlender).ToObservable())
                .Switch();

            var onArm = OnArm
                .WithLatestFrom(states, (a, s) => s.State == IdleState ? Return(a) : Empty<Unit>())
                .Switch();

            var onDisarm = OnDisarm
                .WithLatestFrom(states, (a, s) => s.State == SwingState ? Return(a) : Empty<Unit>())
                .Switch();

            var conflictingInputs = onArm.Select(_ => ConflictingInputs.Freeze());

            var disposed = Disposed.Where(identity);

            // Change animation.
            onArm
                .Select(_ => seeker.TakeUntil(onDisarm))
                .Switch()
                .TakeUntil(disposed)
                .Subscribe(s => s.Animation = SwingAnimation, this);

            // Change animation state.
            onArm
                .Select(_ => states)
                .Switch()
                .TakeUntil(disposed)
                .Do(_ => Logger.LogDebug("Entering armed state."))
                .Subscribe(s => s.State = SwingState, this);

            onArm
                .Select(_ => onDisarm)
                .Switch()
                .Select(_ => states)
                .Switch()
                .TakeUntil(disposed)
                .Do(_ => Logger.LogDebug("Exiting armed state."))
                .Subscribe(s => s.State = IdleState, this);

            // Lock view rotation.
            conflictingInputs
                .Do(_ => Logger.LogDebug("Deactivating conflicting view controls."))
                .Do(i => i.Iter(v => v.Deactivate()))
                .Select(v => onDisarm.Select(_ => v))
                .Switch()
                .Do(_ => Logger.LogDebug("Activating conflicting view controls."))
                .Do(i => i.Iter(v => v.Activate()))
                .TakeUntil(disposed)
                .Subscribe(this);

            // Handle swing motion.
            onArm
                .Select(_ => value.TakeUntil(onDisarm))
                .Switch()
                .SelectMany(v => seeker, (v, s) => (v, s))
                .TakeUntil(disposed)
                .Do(t => Logger.LogDebug("Changing animation position: {}.", t.v))
                .Subscribe(t => t.s.Position = t.v, this);

            // Blend/unblend holding animation.
            onArm
                .Select(_ => blender.TakeUntil(onDisarm))
                .Switch()
                .Do(_ => Logger.LogDebug("Overriding default animation: '{}'.", Animation))
                .TakeUntil(disposed)
                .Subscribe(b => b.Unblend(AnimationTransition), this);

            onArm
                .Select(_ => onDisarm)
                .Switch()
                .Select(_ => blender)
                .Switch()
                .Do(_ => Logger.LogDebug("Restoring default animation: '{}'.", Animation))
                .TakeUntil(disposed)
                .Subscribe(
                    b => Animation.Iter(a => b.Blend(a, transition: AnimationTransition)),
                    this);
        }
    }
}
