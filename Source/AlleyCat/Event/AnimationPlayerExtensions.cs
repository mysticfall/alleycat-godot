using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public static class AnimationPlayerExtensions
    {
        private const string NodeName = "AnimationPlayerTracker";

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
    }
}
