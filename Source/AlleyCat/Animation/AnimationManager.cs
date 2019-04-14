using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationManager : GameObject, IAnimationManager
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public AnimationPlayer Player { get; }

        public IObservable<Unit> OnBeforeAdvance => _onBeforeAdvance.AsObservable();

        public IObservable<float> OnAdvance => _onAdvance.AsObservable();

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        public virtual IObservable<IAnimationEvent> OnAnimationEvent => _onAnimationEvent.AsObservable();

        private readonly BehaviorSubject<bool> _active;

        private readonly ISubject<Unit> _onBeforeAdvance;

        private readonly ISubject<float> _onAdvance;

        private readonly ISubject<IAnimationEvent> _onAnimationEvent;

        public AnimationManager(
            AnimationPlayer player,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(player, nameof(player)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Player = player;
            ProcessMode = processMode;
            TimeSource = timeSource;

            Player.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Manual;

            _active = CreateSubject(active);
            _onBeforeAdvance = CreateSubject<Unit>();
            _onAdvance = CreateSubject<float>();
            _onAnimationEvent = CreateSubject<IAnimationEvent>();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            TimeSource.OnProcess(ProcessMode)
                .Where(_ => Active)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(Advance, this);
        }

        public virtual void Advance(float delta)
        {
            _onBeforeAdvance.OnNext(Unit.Default);

            ProcessFrames(delta);

            _onAdvance.OnNext(delta);
        }

        protected virtual void ProcessFrames(float delta) => Player.Advance(delta);

        public virtual void Play(Godot.Animation animation)
        {
            this.LogDebug("Playing animation: '{}'.", animation);

            Player.Play(Player.AddAnimation(animation));
        }

        public void FireEvent(IAnimationEvent @event)
        {
            this.LogDebug("Received animation event: '{}'.", @event);

            _onAnimationEvent.OnNext(@event);
        }
    }
}
