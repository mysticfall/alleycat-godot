using System;
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

        //FIXME A temporary workaround until godotengine/godot#17623 gets fixed.
        public virtual IObservable<float> OnAdvance => OnLoop;

        public virtual IObservable<AnimationEvent> OnAnimationEvent => _onAnimationEvent;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private readonly Subject<AnimationEvent> _onAnimationEvent = new Subject<AnimationEvent>();

        public AnimationManager()
        {
            ProcessMode = ProcessMode.Idle;
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Player.PlaybackActive = false;

            OnActiveStateChange
                .Subscribe(Player.SetActive)
                .AddTo(this);
        }

        public virtual void Play(Godot.Animation animation)
        {
            Ensure.Any.IsNotNull(animation, nameof(animation));

            var name = animation.GetName();

            if (!Player.HasAnimation(name))
            {
                Player.AddAnimation(name, animation).ThrowIfNecessary();
            }

            Player.Play(name);
        }

        [UsedImplicitly]
        public void FireEvent(string name) => FireEvent(name, null);

        [UsedImplicitly]
        public void FireEvent(string name, string argument)
        {
            _onAnimationEvent.OnNext(new AnimationEvent(name, argument, this));
        }

        protected override void OnPreDestroy()
        {
            _active?.Dispose();

            _onAnimationEvent?.OnCompleted();
            _onAnimationEvent?.Dispose();

            base.OnPreDestroy();
        }
    }
}
