using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public static class AnimationPlayerExtensions
    {
        private const string NodeName = "AnimationPlayerEventTracker";

        public static string AddAnimation(
            [NotNull] this AnimationPlayer player, [NotNull] Godot.Animation animation)
        {
            Ensure.Any.IsNotNull(player, nameof(player));
            Ensure.Any.IsNotNull(animation, nameof(animation));

            var name = animation.GetKey();

            if (!player.HasAnimation(name))
            {
                player.AddAnimation(name, animation).ThrowIfNecessary();
            }

            return name;
        }

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
