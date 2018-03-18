using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class AnimationPlayerEventTracker : EventTracker<AnimationPlayer>
    {
        private const string SignalOnAnimationChange = "animation_changed";

        private const string SignalOnAnimationStart = "animation_started";

        private const string SignalOnAnimationFinish = "animation_finished";

        [NotNull]
        public IObservable<AnimationStartEvent> OnAnimationStart
        {
            get
            {
                if (_onAnimationStart == null)
                {
                    Parent.Connect(SignalOnAnimationStart, this, "FireOnAnimationStart");

                    _onAnimationStart = new Subject<AnimationStartEvent>();
                }

                return _onAnimationStart;
            }
        }

        [NotNull]
        public IObservable<AnimationFinishEvent> OnAnimationFinish
        {
            get
            {
                if (_onAnimationFinish == null)
                {
                    Parent.Connect(SignalOnAnimationFinish, this, "FireOnAnimationFinish");

                    _onAnimationFinish = new Subject<AnimationFinishEvent>();
                }

                return _onAnimationFinish;
            }
        }

        [NotNull]
        public IObservable<AnimationChangeEvent> OnAnimationChange
        {
            get
            {
                if (_onAnimationChange == null)
                {
                    Parent.Connect(SignalOnAnimationChange, this, "FireOnAnimationChange");

                    _onAnimationChange = new Subject<AnimationChangeEvent>();
                }

                return _onAnimationChange;
            }
        }

        private Subject<AnimationChangeEvent> _onAnimationChange;

        private Subject<AnimationStartEvent> _onAnimationStart;

        private Subject<AnimationFinishEvent> _onAnimationFinish;

        [UsedImplicitly]
        private void FireOnAnimationChange(string oldName, string newName) =>
            _onAnimationChange?.OnNext(new AnimationChangeEvent(newName, oldName, Parent));

        [UsedImplicitly]
        private void FireOnAnimationStart(string name) =>
            _onAnimationStart?.OnNext(new AnimationStartEvent(name, Parent));

        [UsedImplicitly]
        private void FireOnAnimationFinish(string name) =>
            _onAnimationFinish?.OnNext(new AnimationFinishEvent(name, Parent));

        protected override void Disconnect(AnimationPlayer parent)
        {
            base.Disconnect(parent);

            Ensure.Any.IsNotNull(parent, nameof(parent));

            if (_onAnimationChange != null)
            {
                parent.Disconnect(SignalOnAnimationChange, this, "FireOnAnimationChange");

                _onAnimationChange.Dispose();
                _onAnimationChange = null;
            }

            if (_onAnimationStart != null)
            {
                parent.Disconnect(SignalOnAnimationStart, this, "FireOnAnimationStart");

                _onAnimationStart.Dispose();
                _onAnimationStart = null;
            }

            if (_onAnimationFinish != null)
            {
                parent.Disconnect(SignalOnAnimationFinish, this, "FireOnAnimationFinish");

                _onAnimationFinish.Dispose();
                _onAnimationFinish = null;
            }
        }
    }
}
