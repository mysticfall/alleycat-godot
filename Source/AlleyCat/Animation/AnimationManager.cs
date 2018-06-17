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

        public IObservable<AnimationEvent> OnAnimationEvent => _onAnimationEvent;

        protected bool PlayingOneShotAnimation { get; private set; }
       
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
            Player.PlaybackActive = false;

            OnLoop
                .Where(_ => Active)
                .Subscribe(Advance)
                .AddTo(this);
        }

        public virtual void Advance(float delta)
        {
            _onBeforeAdvance.OnNext(Unit.Default);

            if (!PlayingOneShotAnimation)
            {
                ProcessFrames(delta);
            }

            _onAdvance.OnNext(delta);
        }

        public virtual void Play(string animation, System.Action onFinish = null)
        {
            Ensure.Any.IsNotNull(animation, nameof(animation));

            PlayingOneShotAnimation = true;
            Player.PlaybackActive = true;

            void Reset(bool invokeCallback)
            {
                PlayingOneShotAnimation = false;
                Player.PlaybackActive = false;

                if (invokeCallback)
                {
                    onFinish?.Invoke();
                }
            }

            try
            {
                Player.Play(animation);

                Player.OnAnimationFinish()
                    .Where(e => e.Animation == animation)
                    .AsUnitObservable()
                    .Merge(
                        Player.OnAnimationChange()
                            .Where(e => e.OldAnimation == animation)
                            .AsUnitObservable())
                    .Subscribe(_ => Reset(true), _ => Reset(false))
                    .AddTo(this);
            }
            catch (Exception)
            {
                Reset(false);
                throw;
            }
        }

        [UsedImplicitly]
        public void FireEvent(string name) => FireEvent(name, null);

        [UsedImplicitly]
        public void FireEvent(string name, string argument)
        {
            _onAnimationEvent.OnNext(new AnimationEvent(name, argument, this));
        }

        protected virtual void ProcessFrames(float delta) => Player.Advance(delta);

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();

            _onBeforeAdvance?.OnCompleted();
            _onBeforeAdvance?.Dispose();

            _onAdvance?.OnCompleted();
            _onAdvance?.Dispose();

            _onAnimationEvent?.OnCompleted();
            _onAnimationEvent?.Dispose();

            base.Dispose(disposing);
        }
    }
}
