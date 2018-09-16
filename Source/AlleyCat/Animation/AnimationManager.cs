using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

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

        [Service]
        public AnimationPlayer Player { get; private set; }

        public IObservable<Unit> OnBeforeAdvance => _onBeforeAdvance;

        public IObservable<float> OnAdvance => _onAdvance;

        public virtual IObservable<AnimationEvent> OnAnimationEvent => _onAnimationEvent;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private readonly Subject<Unit> _onBeforeAdvance = new Subject<Unit>();

        private readonly Subject<float> _onAdvance = new Subject<float>();

        private readonly Subject<AnimationEvent> _onAnimationEvent = new Subject<AnimationEvent>();

        public AnimationManager()
        {
            ProcessMode = ProcessMode.Idle;
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
            Ensure.Any.IsNotNull(animation, nameof(animation));

            var name = Player.AddAnimation(animation);

            Player.Play(name);
        }

        [UsedImplicitly]
        public void FireEvent(string name) => FireEvent(name, null);

        [UsedImplicitly]
        public void FireEvent(string name, object argument)
        {
            _onAnimationEvent.OnNext(new AnimationEvent(name, argument, this));
        }

        protected override void OnPreDestroy()
        {
            _active?.Dispose();

            _onBeforeAdvance?.OnCompleted();
            _onBeforeAdvance?.Dispose();

            _onAdvance?.OnCompleted();
            _onAdvance?.Dispose();

            _onAnimationEvent?.OnCompleted();
            _onAnimationEvent?.Dispose();

            base.OnPreDestroy();
        }
    }
}
