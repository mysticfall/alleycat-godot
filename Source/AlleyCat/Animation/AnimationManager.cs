using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    [AutowireContext, Singleton(typeof(IAnimationManager))]
    public class AnimationManager : AutowiredNode, IAnimationManager
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public AnimationPlayer Player => (AnimationPlayer) _player;

        public IObservable<Unit> OnBeforeAdvance => _onBeforeAdvance;

        public IObservable<float> OnAdvance => _onAdvance;

        public virtual IObservable<AnimationEvent> OnAnimationEvent => _onAnimationEvent;

        [Service] private Option<AnimationPlayer> _player = None;

        private readonly ReactiveProperty<bool> _active;

        private readonly ISubject<Unit> _onBeforeAdvance;

        private readonly ISubject<float> _onAdvance;

        private readonly ISubject<AnimationEvent> _onAnimationEvent;

        public AnimationManager()
        {
            ProcessMode = ProcessMode.Idle;

            _active = new ReactiveProperty<bool>(true).AddTo(this);
            _onBeforeAdvance = new Subject<Unit>().AddTo(this);
            _onAdvance = new Subject<float>().AddTo(this);
            _onAnimationEvent = new Subject<AnimationEvent>().AddTo(this);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Player.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Manual;

            OnLoop
                .Where(_ => Active)
                .Subscribe(Advance)
                .AddTo(this);
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
            Ensure.That(animation, nameof(animation)).IsNotNull();

            Player.Play(Player.AddAnimation(animation));
        }

        [UsedImplicitly]
        public void FireEvent(string name) => FireEvent(name, None);

        [UsedImplicitly]
        public void FireEvent(string name, [CanBeNull] object argument)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            _onAnimationEvent.OnNext(new AnimationEvent(name, Optional(argument), this));
        }
    }
}
