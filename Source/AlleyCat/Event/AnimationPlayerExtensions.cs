using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public interface IAnimationPlayerEvent : IEvent<AnimationPlayer>
    {
        [NotNull]
        string Animation { get; }
    }

    public struct AnimationStartEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        public AnimationPlayer Source { get; }

        public AnimationStartEvent([NotNull] string animation, [NotNull] AnimationPlayer source)
        {
            Ensure.String.IsNotNullOrWhiteSpace(animation, nameof(animation));
            Ensure.Any.IsNotNull(source, nameof(source));

            Animation = animation;
            Source = source;
        }
    }

    public struct AnimationFinishEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        public AnimationPlayer Source { get; }

        public AnimationFinishEvent([NotNull] string animation, [NotNull] AnimationPlayer source)
        {
            Ensure.String.IsNotNullOrWhiteSpace(animation, nameof(animation));
            Ensure.Any.IsNotNull(source, nameof(source));

            Animation = animation;
            Source = source;
        }
    }

    public struct AnimationChangeEvent : IAnimationPlayerEvent
    {
        public string Animation { get; }

        [NotNull]
        public string OldAnimation { get; }

        public AnimationPlayer Source { get; }

        public AnimationChangeEvent(
            [NotNull] string animation,
            [NotNull] string oldAnimation,
            [NotNull] AnimationPlayer source)
        {
            Ensure.String.IsNotNullOrWhiteSpace(animation, nameof(animation));
            Ensure.String.IsNotNullOrWhiteSpace(oldAnimation, nameof(oldAnimation));

            Ensure.Any.IsNotNull(source, nameof(source));

            Animation = animation;
            OldAnimation = oldAnimation;
            Source = source;
        }
    }

    public static class AnimationPlayerExtensions
    {
        private const string NodeName = "AnimationPlayerTracker";

        private const string SignalOnAnimationChange = "animation_changed";

        private const string SignalOnAnimationStart = "animation_started";

        private const string SignalOnAnimationFinish = "animation_finished";

        [NotNull]
        public static IObservable<AnimationChangeEvent> OnAnimationChange(
            [NotNull] this AnimationPlayer player)
        {
            Ensure.Any.IsNotNull(player, nameof(player));

            var tracker = player.GetOrCreateNode(NodeName, _ => new AnimationPlayerEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnAnimationChange;
        }

        [NotNull]
        public static IObservable<AnimationStartEvent> OnAnimationStart(
            [NotNull] this AnimationPlayer player)
        {
            Ensure.Any.IsNotNull(player, nameof(player));

            var tracker = player.GetOrCreateNode(NodeName, _ => new AnimationPlayerEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnAnimationStart;
        }

        [NotNull]
        public static IObservable<AnimationFinishEvent> OnAnimationFinish(
            [NotNull] this AnimationPlayer player)
        {
            Ensure.Any.IsNotNull(player, nameof(player));

            var tracker = player.GetOrCreateNode(NodeName, _ => new AnimationPlayerEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnAnimationFinish;
        }

        public class AnimationPlayerEventTracker : EventTracker<AnimationPlayer>
        {
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
}
