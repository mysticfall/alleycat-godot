using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationPlayerEventTracker : EventTracker<AnimationPlayer>
    {
        private const string SignalOnAnimationChange = "animation_changed";

        private const string SignalOnAnimationStart = "animation_started";

        private const string SignalOnAnimationFinish = "animation_finished";

        public IObservable<AnimationStartEvent> OnAnimationStart => _onAnimationStart.Head().AsObservable();

        public IObservable<AnimationFinishEvent> OnAnimationFinish => _onAnimationFinish.Head().AsObservable();

        public IObservable<AnimationChangeEvent> OnAnimationChange => _onAnimationChange.Head().AsObservable();

        private Option<Subject<AnimationChangeEvent>> _onAnimationChange;

        private Option<Subject<AnimationStartEvent>> _onAnimationStart;

        private Option<Subject<AnimationFinishEvent>> _onAnimationFinish;

        public override void _EnterTree()
        {
            base._EnterTree();

            var parent = (AnimationPlayer) Parent;

            _onAnimationStart = Some(_ =>
            {
                parent.Connect(SignalOnAnimationStart, this, nameof(FireOnAnimationStart));

                return new Subject<AnimationStartEvent>();
            });

            _onAnimationChange = Some(_ =>
            {
                parent.Connect(SignalOnAnimationChange, this, nameof(FireOnAnimationChange));

                return new Subject<AnimationChangeEvent>();
            });

            _onAnimationFinish = Some(_ =>
            {
                parent.Connect(SignalOnAnimationFinish, this, nameof(FireOnAnimationFinish));

                return new Subject<AnimationFinishEvent>();
            });
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            _onAnimationStart = None;
            _onAnimationChange = None;
            _onAnimationFinish = None;
        }

        [UsedImplicitly]
        private void FireOnAnimationStart(string name)
        {
            Debug.Assert(name != null, "name != null");

            Parent
                .SelectMany(parent => _onAnimationStart, (parent, observable) => (parent, observable))
                .Iter(t => { t.observable.OnNext(new AnimationStartEvent(name, t.parent)); });
        }

        [UsedImplicitly]
        private void FireOnAnimationChange(Option<string> oldName, string newName)
        {
            Debug.Assert(oldName != null, "oldName != null");
            Debug.Assert(newName != null, "newName != null");

            Parent
                .SelectMany(parent => _onAnimationChange, (parent, subject) => (parent, subject))
                .Iter(t => t.subject.OnNext(new AnimationChangeEvent(newName, oldName, t.parent)));
        }

        [UsedImplicitly]
        private void FireOnAnimationFinish(string name)
        {
            Debug.Assert(name != null, "name != null");

            Parent
                .SelectMany(parent => _onAnimationFinish, (parent, observable) => (parent, observable))
                .Iter(t => { t.observable.OnNext(new AnimationFinishEvent(name, t.parent)); });
        }

        protected override void Disconnect(AnimationPlayer parent)
        {
            base.Disconnect(parent);

            if (_onAnimationChange)
            {
                parent.Disconnect(SignalOnAnimationChange, this, nameof(FireOnAnimationChange));

                _onAnimationChange.Iter(v => v.DisposeQuietly());
                _onAnimationChange = None;
            }

            if (_onAnimationStart)
            {
                parent.Disconnect(SignalOnAnimationStart, this, nameof(FireOnAnimationStart));

                _onAnimationStart.Iter(v => v.DisposeQuietly());
                _onAnimationStart = None;
            }

            if (_onAnimationFinish)
            {
                parent.Disconnect(SignalOnAnimationFinish, this, nameof(FireOnAnimationFinish));

                _onAnimationFinish.Iter(v => v.DisposeQuietly());
                _onAnimationFinish = None;
            }
        }
    }
}
