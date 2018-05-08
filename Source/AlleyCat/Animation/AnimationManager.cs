using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager))]
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

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private readonly Subject<Unit> _onBeforeAdvance = new Subject<Unit>();

        private readonly Subject<float> _onAdvance = new Subject<float>();

        public AnimationManager()
        {
            ProcessMode = ProcessMode.Idle;
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Player.PlaybackActive = false;

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

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();

            _onBeforeAdvance?.OnCompleted();
            _onBeforeAdvance?.Dispose();

            _onAdvance?.OnCompleted();
            _onAdvance?.Dispose();

            base.Dispose(disposing);
        }
    }
}
