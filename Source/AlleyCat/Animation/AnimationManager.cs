using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

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

        public virtual IObservable<AnimationEvent> OnAnimationEvent => _onAnimationEvent.AsObservable();

        private readonly BehaviorSubject<bool> _active;

        private readonly ISubject<Unit> _onBeforeAdvance;

        private readonly ISubject<float> _onAdvance;

        private readonly ISubject<AnimationEvent> _onAnimationEvent;

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

            _active = new BehaviorSubject<bool>(active).DisposeWith(this);
            _onBeforeAdvance = new Subject<Unit>().DisposeWith(this);
            _onAdvance = new Subject<float>().DisposeWith(this);
            _onAnimationEvent = new Subject<AnimationEvent>().DisposeWith(this);

            Player.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Manual;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            TimeSource.OnProcess(ProcessMode)
                .Where(_ => Active)
                .Subscribe(Advance)
                .DisposeWith(this);
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

        public void FireEvent(string name, Option<object> argument)
        {
            this.LogDebug("Received animation event: '{}' (args: {}).", name, argument);

            _onAnimationEvent.OnNext(new AnimationEvent(name, argument, this));
        }
    }
}
